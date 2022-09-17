using System.Collections.Generic;
using System.IO;

namespace NanopassSharp;

/// <summary>
/// A context for an execution.
/// </summary>
/// <param name="Directory">The root directory of the current execution.</param>
/// <param name="Args">The argument passed to the execution.</param>
public readonly record struct ExecutionContext(
    DirectoryInfo Directory,
    IReadOnlyDictionary<string, string> Args
);
