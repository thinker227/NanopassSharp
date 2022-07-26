﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoreLinq;
using Scriban;
using NanopassSharp.Functional;
using NanopassSharp.Models;

namespace NanopassSharp;

internal static class PassSourceGenerator {

	public readonly record struct ModifiedTypeResult(
		string Source, NamespacedTypeName TypeName);
	public static Result<ModifiedTypeResult> GetModifiedTypeSource(RecordDeclarationSyntax baseSyntax, INamedTypeSymbol baseType, PassModel pass, ModificationPassModel mod) {
		var typeName = GetNamespacedTypeName(pass, mod, baseType);

		var root = GetRootPassRecord(baseSyntax, baseType, pass, mod);

		var template = TemplateResult.Value;
		if (!template.IsSuccess) return new(template.Error);

		string source = RenderSource(template.Value, typeName.Namespace, root);

		return new ModifiedTypeResult(source, GetNamespacedTypeName(pass, mod, baseType));
	}

	private static Lazy<Result<Template>> TemplateResult { get; } = new(GetTemplate);
	private static Result<Template> GetTemplate() {
		const string templateResourcePath = "NanopassSharp.template.sbncs";
		var assembly = System.Reflection.Assembly.GetExecutingAssembly();
		var resourceStream = assembly.GetManifestResourceStream(templateResourcePath);
		if (resourceStream is null) {
			return $"Could not read resource stream of resource '{templateResourcePath}'";
		}
		StreamReader reader = new(resourceStream);
		string templateSource = reader.ReadToEnd();

		var template = Template.Parse(templateSource);
		if (template.HasErrors) {
			return template.Messages.ToString();
		}

		return template;
	}
	private static string RenderSource(Template template, string @namespace, PassRecord root) {
		var context = new {
			Namespace = @namespace,
			Root = root,
		};

		string source = template.Render(context);

		return source;
	}

	public readonly record struct NamespacedTypeName(string Name, string Namespace) {
		public string FullName { get; } = $"{Namespace}.{Name}";
	}
	public static NamespacedTypeName GetNamespacedTypeName(PassModel pass, ModificationPassModel mod, INamedTypeSymbol baseType) {
		string @namespace = baseType.GetFullNamespace();
		string typeName = GetTypeName(pass, mod);
		return new(typeName, @namespace);
	}
	public static string GetTypeName(PassModel pass, ModificationPassModel mod) =>
		mod.TypeName ?? pass.Name;

	private readonly record struct TypeMod(
		INamedTypeSymbol Type,
		ImmutableArray<ModificationModel> Add,
		ImmutableArray<ModificationModel> Remove
	);
	private static IReadOnlyDictionary<INamedTypeSymbol, TypeMod> GetTypeMods(INamedTypeSymbol baseType, ModificationPassModel mod) {
		var paths = GetTargetPaths(baseType);
		var mods = GetTargetedTypeMods(mod);
		return mods
			.Join(
				paths,
				m => m.Target,
				p => p.Target,
				(m, p) => new TypeMod(p.Type, m.Add, m.Remove)
			).ToDictionary<TypeMod, INamedTypeSymbol>(m => m.Type, SymbolEqualityComparer.Default);
	}
	private readonly record struct TargetedTypeMod(
		string Target,
		ImmutableArray<ModificationModel> Add,
		ImmutableArray<ModificationModel> Remove
	);
	private static IEnumerable<TargetedTypeMod> GetTargetedTypeMods(ModificationPassModel modPass) {
		var add = modPass.Add
			.GroupBy(a => a.Target);
		var remove = modPass.Remove
			.GroupBy(r => r.Target);

		return add.FullJoin(
			remove,
			m => m.Key,
			a => new TargetedTypeMod(a.Key, a.ToImmutableArray(), ImmutableArray<ModificationModel>.Empty),
			r => new TargetedTypeMod(r.Key, ImmutableArray<ModificationModel>.Empty, r.ToImmutableArray()),
			(a, r) => new TargetedTypeMod(a.Key, a.ToImmutableArray(), r.ToImmutableArray())
		).ToArray();
	}

	private readonly record struct TypeAndTargetPath(
		INamedTypeSymbol Type,
		string Target
	);
	private static IEnumerable<TypeAndTargetPath> GetTargetPaths(INamedTypeSymbol baseType) =>
		GetTargetPaths(baseType, "").Prepend_(new(baseType, "this"));
	private static IEnumerable<TypeAndTargetPath> GetTargetPaths(INamedTypeSymbol type, string currentPath) =>
		type.GetTypeMembers()
			.SelectMany(t => GetTargetPaths(t, currentPath == "" ? t.Name : $"{currentPath}.{t.Name}"));

	private sealed record class PassRecord(
		PassRecord? Parent,
		string Name,
		List<string> Parameters,
		List<string> Properties,
		HashSet<PassRecord> Nested
	) {
		
	}
	private static PassRecord GetRootPassRecord(RecordDeclarationSyntax baseSyntax, INamedTypeSymbol baseType, PassModel pass, ModificationPassModel mod) {
		var mods = GetTypeMods(baseType, mod);
		var rootRecord = GetPassRecord(baseSyntax, baseType, mods, null);
		return rootRecord with {
			Name = GetTypeName(pass, mod)
		};
	}
	private static PassRecord GetPassRecord(RecordDeclarationSyntax baseSyntax, INamedTypeSymbol baseType, IReadOnlyDictionary<INamedTypeSymbol, TypeMod> mods, PassRecord? parent) {
		string name = baseType.Name;

		var mod = mods.TryGetValue(baseType, out var m) ? m : (TypeMod?)null;
		var add = mod?.Add ?? ImmutableArray<ModificationModel>.Empty;
		var remove = mod?.Remove ?? ImmutableArray<ModificationModel>.Empty;

		var removeParams = remove
			.Select(r => r.Parameter)
			.NotNull()
			.ToHashSet_();
		var removeProps = remove
			.Select(r => r.Property)
			.NotNull()
			.ToHashSet_();
		var removeTypes = remove
			.Select(r => r.Type)
			.NotNull()
			.ToHashSet_();
		
		var baseParams = baseSyntax.ParameterList is not null
			? baseSyntax.ParameterList.Parameters
				.Where(p => !removeParams.Contains(p.Identifier.Text))
				.Select(p => p.GetTextWithoutTrivia())
			: Enumerable.Empty<string>();
		var modParams = add
			.Select(a => a.Parameter)
			.OfType<string>();
		var parameters = baseParams
			.Concat(modParams)
			.ToList();

		var baseProperties = baseSyntax.Members
			.OfType<PropertyDeclarationSyntax>()
			.Where(p => !removeProps.Contains(p.Identifier.Text))
			.Select(p => p.GetTextWithoutTrivia());
		var modProperties = add
			.Select(a => a.Property)
			.OfType<string>();
		var properties = baseProperties
			.Concat(modProperties)
			.ToList();

		PassRecord record = new(parent, name, parameters, properties, new HashSet<PassRecord>());

		var nested = baseType.GetTypeMembers()
			.Where(t => t.IsRecord)
			.Where(t => !removeTypes.Contains(t.Name))
			.Select(t => GetPassRecord(
				(RecordDeclarationSyntax)t.DeclaringSyntaxReferences[0].GetSyntax(),
				t,
				mods,
				record
			));
		foreach (var n in nested) record.Nested.Add(n);

		return record;
	}
	
}
