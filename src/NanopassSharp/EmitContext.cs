using System.Collections.Generic;
using System.IO;

namespace NanopassSharp;

/// <summary>
/// A context for an emit operation.
/// </summary>
/// <param name="Hierarchy">The hierarchy which should be emitted.</param>
/// <param name="Directory">The root directory of the operation.</param>
/// <param name="Args">The arguments passed to the program.</param>
public readonly record struct EmitContext(
    AstNodeHierarchy Hierarchy,
    DirectoryInfo Directory,
    IReadOnlyDictionary<string, string> Args
);
