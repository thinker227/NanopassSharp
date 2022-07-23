using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NanopassSharp.Functional;

namespace NanopassSharp;

public static class RoslynExtensions {

	public static IEnumerable<INamedTypeSymbol> GetAllTypes(this INamespaceSymbol source) =>
		source
			.GetTypeMembers()
			.Concat(source.GetNamespaceMembers()
				.SelectMany(n => n.GetAllTypes()));

	public static async Task<Result<Compilation>> GetCompilationResultAsync(this Project project) =>
		Result.CreateNotNull(await project.GetCompilationAsync(), "Retrieved compilation was null");

	public static string GetFullNamespace(this INamespaceOrTypeSymbol symbol) =>
		GetFullNamespace(symbol.ContainingNamespace, new()).ToString();
	private static StringBuilder GetFullNamespace(INamespaceSymbol symbol, StringBuilder builder) {
		if (symbol.ContainingNamespace is not null && !symbol.ContainingNamespace.IsGlobalNamespace) {
			GetFullNamespace(symbol.ContainingNamespace, builder);
			builder.Append('.');
		}
		builder.Append(symbol.Name);
		return builder;
	}

	public static IEnumerable<INamedTypeSymbol> GetAllNestedTypes(this INamedTypeSymbol source) =>
		source.GetTypeMembers()
			.SelectMany(t => t.GetAllNestedTypes());

	public static string GetTextWithoutTrivia(this SyntaxNode node) =>
		node.WithoutTrivia().GetText().ToString();

}
