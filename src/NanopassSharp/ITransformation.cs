namespace NanopassSharp;

/// <summary>
/// A description of a transformation and pattern.
/// </summary>
public interface ITransformationDescription
{
    /// <summary>
    /// The pattern the transformation is triggered by.
    /// </summary>
    ITransformationPattern? Pattern { get; }
    /// <summary>
    /// The transformation to apply.
    /// </summary>
    ITransformation Transformation { get; }
}

/// <summary>
/// A transformation on a node or node member.
/// </summary>
public interface ITransformation
{
    /// <summary>
    /// Applies the transformation to a tree.
    /// </summary>
    /// <param name="tree">The tree to apply the transformation to.</param>
    /// <returns>A new tree with the applied transformation.</returns>
    AstNodeHierarchy ApplyToTree(AstNodeHierarchy tree);
    /// <summary>
    /// Applies the transformation to a node tree.
    /// </summary>
    /// <param name="tree">The tree the node is a part of.</param>
    /// <param name="node">The node to apply the transformation to.</param>
    /// <returns>A new node with the applied transformation.</returns>
    AstNodeHierarchy ApplyToNode(AstNodeHierarchy tree, AstNode node);
    /// <summary>
    /// Applies the transformation to a node member.
    /// </summary>
    /// <param name="tree">The tree the node of the member is a part of.</param>
    /// <param name="node">The node the member is a part of.</param>
    /// <param name="member">The member to apply the transformation to.</param>
    /// <returns>A new member with the applied transformation.</returns>
    AstNodeHierarchy ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member);
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
    /// Returns whether a tree matches the pattern.
    /// </summary>
    /// <param name="tree">The tree to check.</param>
    bool IsMatch(AstNodeHierarchy tree);
    /// <summary>
    /// Returns whether a node matches the pattern.
    /// </summary>
    /// <param name="node">The node to check.</param>
    bool IsMatch(AstNode node);
    /// <summary>
    /// Returns whether a member of a node matches the pattern.
    /// </summary>
    /// <param name="node">The node the member is a part of.</param>
    /// <param name="member">The node member to check.</param>
    bool IsMatch(AstNode node, AstNodeMember member);
}
