using System.Collections.Generic;
using System.Linq;
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

}
