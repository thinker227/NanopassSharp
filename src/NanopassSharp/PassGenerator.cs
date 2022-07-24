using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using NanopassSharp.Models;
using NanopassSharp.Functional;

namespace NanopassSharp;

public sealed class PassGenerator {

	private const string nanopassSharpNamespace = "NanopassSharp";
	private const string passAttributeName = "PassAttribute";
	private const string passAttributeFullName = $"{nanopassSharpNamespace}.{passAttributeName}";

	private readonly IReadOnlyList<PassModel> models;



	private PassGenerator(IReadOnlyList<PassModel> models) {
		this.models = models;
	}



	public static Task<PassGenerator> CreateAsync(IReadOnlyList<PassModel> models) {
		PassGenerator generator = new(models);
		return Task.FromResult(generator);
	}

	public async Task<string> RunAsync(string projectPath) {
		var (project, workspace) = await GetProjectAsync(projectPath);
		using (workspace) {
			var result = await RunPassesAsync(null, project, models);
			return result.Switch(
				value => workspace.TryApplyChanges(value.Project.Solution)
					? $"Successfully applied changes to project '{value.Project.Name}'"
					: $"Failed to apply changes to project '{value.Project.Name}'",
				error => error
			);
		}	
	}

	private readonly record struct GetProjectResult(Project Project, Workspace Workspace);
	/// <summary>
	/// Gets a <see cref="Project"/> from a path.
	/// </summary>
	private static async Task<GetProjectResult> GetProjectAsync(string projectPath) {
		MSBuildLocator.RegisterDefaults();
		var workspace = MSBuildWorkspace.Create();
		var project = await workspace.OpenProjectAsync(projectPath);
		return new(project, workspace);
	}

	private readonly record struct PassResult(INamedTypeSymbol? Type, Project Project);
	/// <summary>
	/// Runs a sequence of passes.
	/// </summary>
	private static async Task<Result<PassResult>> RunPassesAsync(INamedTypeSymbol? baseType, Project project, IEnumerable<PassModel> models) => models.Any()
		? await GeneratePassAsync(project, baseType, models.First())
			.BindResultAsync(v => RunPassesAsync(v.Type, v.Project, models.Skip(1)))
		: new();
	/// <summary>
	/// Generates a pass.
	/// </summary>
	private static async ValueTask<Result<PassResult>> GeneratePassAsync(Project project, INamedTypeSymbol? baseType, PassModel model) =>
		(baseType, model) switch {
			(_, { Type: not null and var type }) => await GenerateTypePassAsync(project, type),
			(null, { Mod: not null }) => $"Pass {model.Name}: no base type",
			(_, { Mod: not null and var mod }) => await GenerateModPassAsync(project, baseType, model, mod),
			_ => $"Pass {model.Name}: invalid pass"
		};
	/// <summary>
	/// Generates a type pass.
	/// </summary>
	private static async Task<Result<PassResult>> GenerateTypePassAsync(Project project, string typeName) {
		var compilation = await project.GetCompilationAsync();
		if (compilation is null) return "Failed to retrieve compilation";
		var type = compilation.GetTypeByMetadataName(typeName);
		if (type is null) return $"Type '{typeName}' does not exist";
		return new PassResult(type, project);
	}
	/// <summary>
	/// Generates a modification pass.
	/// </summary>
	private static async Task<Result<PassResult>> GenerateModPassAsync(Project project, INamedTypeSymbol baseType, PassModel pass, ModificationPassModel mod) {
		var compilation = await project.GetCompilationAsync();
		if (compilation is null) return "Failed to retrieve compilation";
		var preexistingPassType = await TryGetPreexistingPassTypeAsync(project, pass.Name)
			.ToMaybeDefaultAsync();
		if (preexistingPassType is not null) {
			if (await PassTypeIsUpToDateAsync(preexistingPassType, mod)) {
				return new PassResult(preexistingPassType, project);
			}
			else {
				var newProject = await TryDeleteTypeAsync(project, preexistingPassType);
				if (!newProject.IsSuccess) return $"Failed to delete out-of-date pass '{preexistingPassType.Name}'";
				project = newProject.Value;
			}
		}

		return await GenerateModifiedTypeAsync(project, baseType, pass, mod);
	}

	/// <summary>
	/// Checks whether a pass type is up-to-date with a modification pass.
	/// </summary>
	private static Task<bool> PassTypeIsUpToDateAsync(INamedTypeSymbol type, ModificationPassModel mod) {
		throw new NotImplementedException();
	}
	/// <summary>
	/// Deletes a type from a project.
	/// </summary>
	private static async Task<Result<Project>> TryDeleteTypeAsync(Project project, INamedTypeSymbol type) {
		var syntaxRefs = type.DeclaringSyntaxReferences;
		if (syntaxRefs.Length == 0) {
			return $"Found no syntax references for type '{type.Name}' in project '{project.Name}'";
		}
		foreach (var syntaxRef in syntaxRefs) {
			var document = project.GetDocument(syntaxRef.SyntaxTree)!;
			var root = await document.GetSyntaxRootAsync();
			var syntax = syntaxRef.GetSyntax();
			var newRoot = root!.RemoveNode(syntax, SyntaxRemoveOptions.KeepTrailingTrivia);
			var newDocument = document.WithSyntaxRoot(newRoot!);
			project = newDocument.Project;
		}
		return project;
	}

	private static async Task<Result<INamedTypeSymbol?>> TryGetPreexistingPassTypeAsync(Project project, string passName) {
		var passAttributeResult = await GetPassAttributeAsync(project);
		if (!passAttributeResult.IsSuccess) return new(passAttributeResult.Error);
		var passAttribute = passAttributeResult.Value;

		var compilationResult = await passAttribute.Project.GetCompilationResultAsync();
		if (!compilationResult.IsSuccess) return new(compilationResult.Error);

		var types = compilationResult.Value.SourceModule.GlobalNamespace
			.GetAllTypes()
			.Select(t => (
				type: t,
				name: GetPassName(t, passAttribute.Type)
			))
			.Where(t => t.name.Switch(
				n => n == passName,
				_ => false
			))
			.Select(t => t.type)
			.FirstOrDefault();
		return Result.Success(types);
	}
	private readonly record struct GetPassAttributeResult(Project Project, INamedTypeSymbol Type);
	private static async Task<Result<GetPassAttributeResult>> GetPassAttributeAsync(Project project) =>
		await project.GetCompilationResultAsync()
			.MapResultAsync(c => c.GetTypeByMetadataName(passAttributeFullName))
			.BindResultAsync(async a =>
				a is null
				? await GeneratePassAttributeAsync(project)
				: Result.Success(new GetPassAttributeResult(project, a)));
	private static async Task<Result<GetPassAttributeResult>> GeneratePassAttributeAsync(Project project) {
		string source =
$$"""
// <auto-generated />

namespace {{nanopassSharpNamespace}};

[global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PassAttribute : Attribute {

	public string PassName { get; }

	public PassAttribute(string passName) {
		PassName = passName;
	}
}

""";
		var newProject = project.AddDocument($"{passAttributeName}.g.cs", source).Project;
		return await newProject.GetCompilationResultAsync()
			.MapResultAsync(c => c.GetTypeByMetadataName(passAttributeFullName))
			.NotNullResultAsync($"Failed to retrieve generated type '{passAttributeFullName}'")
			.MapResultAsync(t => new GetPassAttributeResult(newProject, t));
	}
	private static Result<string> GetPassName(INamedTypeSymbol type, INamedTypeSymbol attribute) {
		string[] applicable = type.GetAttributes()
			.Where(a => a.AttributeClass?.Equals(attribute, SymbolEqualityComparer.Default) ?? false)
			.Select(a => a.ConstructorArguments.FirstOrDefault().Value)
			.OfType<string>()
			.ToArray();
		
		if (applicable.Length == 0)
			return Result.Failure<string>($"Type '{type.Name}' has no pass attribute");
		if (applicable.Length > 1)
			return Result.Failure<string>($"Type '{type.Name}' has more than one pass attribute");
		
		return Result.Success(applicable[0]);
	}

	private static async Task<Result<PassResult>> GenerateModifiedTypeAsync(Project project, INamedTypeSymbol baseType, PassModel pass, ModificationPassModel mod) {
		var syntaxRefs = baseType.DeclaringSyntaxReferences;
		
		if (syntaxRefs.Length == 0) return $"Type '{baseType.Name}' has no syntax references and can therefore not be modified";
		if (!baseType.IsRecord) return $"Type '{baseType.Name}' is not a record type - only modifications to record types are supported";
		
		var recordSyntaxes = syntaxRefs
			.Select(s => s.GetSyntax())
			.OfType<RecordDeclarationSyntax>()
			.ToArray();
		var syntax = recordSyntaxes.Length == 1
			? recordSyntaxes[0]
			: MergeRecordDeclarations(recordSyntaxes);

		var modifiedTypeResult = PassSourceGenerator.GetModifiedTypeSource(syntax, baseType, pass, mod);
		if (!modifiedTypeResult.IsSuccess) return new(modifiedTypeResult.Error);
		var (source, typeName) = modifiedTypeResult.Value;

		string fileName = $"{typeName.Name}.g.cs";
		var newProject = project.AddDocument(fileName, source).Project;
		var type = (await newProject.GetCompilationAsync())
			?.GetTypeByMetadataName(typeName.FullName);

		return new PassResult(type, newProject);
	}

	/// <summary>
	/// Merges several <see cref="RecordDeclarationSyntax"/>es into a single syntax.
	/// </summary>
	private static RecordDeclarationSyntax MergeRecordDeclarations(IEnumerable<RecordDeclarationSyntax> records) {
		// This method is incomplete, it does not handle partial methods nor nested partial types
		var attributes = records
			.SelectMany(r => r.AttributeLists);
		var modifiers = records
			.SelectMany(r => r.Modifiers)
			.Where(r => !r.IsKind(SyntaxKind.PartialKeyword))
			.DistinctBy(m => m.Kind())
			.OrderBy(r => r.IsKind(SyntaxKind.RefKeyword) ? 1 : 0);
		var parameterList = records
			.FirstOrDefault(r => r.ParameterList is not null)
			?.ParameterList;
		var members = records
			.SelectMany(r => r.Members);
		return records.First()
			.WithAttributeLists(new(attributes))
			.WithModifiers(new(modifiers))
			.WithParameterList(parameterList)
			.WithMembers(new(members));
	}

}
