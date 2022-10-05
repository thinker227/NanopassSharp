using System.Collections.Generic;

namespace NanopassSharp;

/// <summary>
/// A context for an input.
/// </summary>
/// <param name="Text">The input text.</param>
/// <param name="Args">The arguments passed to the program.</param>
public readonly record struct InputContext(
    string Text,
    IReadOnlyDictionary<string, string> Args
);
