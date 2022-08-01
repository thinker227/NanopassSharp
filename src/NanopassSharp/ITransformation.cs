namespace NanopassSharp;

/// <summary>
/// A transformation on a node tree.
/// </summary>
public interface ITransformation
{

    /// <summary>
    /// The pattern the transformation is triggered by.
    /// </summary>
    ITransformationPattern? Pattern { get; }
    /// <summary>
    /// Applies the transformation onto a node tree.
    /// </summary>
    /// <param name="tree">The node tree to apply the transformation onto.</param>
    /// <returns>A new node tree with the applied transformation.</returns>
    TypeNodeTree Apply(TypeNodeTree tree);

}

/// <summary>
/// A pattern for a transformation.
/// </summary>
public interface ITransformationPattern
{

    /// <summary>
    /// Whether the pattern is recursive.
    /// </summary>
    bool IsRecursive { get; }
    /// <summary>
    /// Returns whether a node matches the pattern.
    /// </summary>
    /// <param name="node">The node to check.</param>
    bool IsMatch(TypeNode node);

}
