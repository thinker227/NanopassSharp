namespace NanopassSharp.Builders;

/// <summary>
/// The behavior if a child node is missing when building a hierarchy.
/// </summary>
public enum MissingChildBehavior
{
    /// <summary>
    /// Throw an exception if a child node is missing.
    /// </summary>
    Throw,
    /// <summary>
    /// Create an empty node if a child node is missing.
    /// </summary>
    CreateEmptyNode
}
