using System.Collections.Generic;
using System.IO;

namespace NanopassSharp;

/// <summary>
/// A context for the initialization of an execution.
/// </summary>
/// <param name="Directory">The root directory of the current execution.</param>
/// <param name="Args">The argument passed to the execution.</param>
public readonly record struct InitializationContext(
    DirectoryInfo Directory,
    IReadOnlyDictionary<string, string> Args
);
