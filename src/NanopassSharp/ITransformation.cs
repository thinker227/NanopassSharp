﻿namespace NanopassSharp;

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
    // Returning null from a tree transformation makes no sense,
    // would mean the tree would disappear, and then what would
    // the consuming code do with that?
    // Nodes and members are fine to remove though, as such an
    // action has well-defined semantics on the resulting tree.

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
    /// <returns>A new node with the applied transformation,
    /// or <see langword="null"/> if the node should be removed.</returns>
    AstNode? ApplyToNode(AstNodeHierarchy tree, AstNode node);
    /// <summary>
    /// Applies the transformation to a node member.
    /// </summary>
    /// <param name="tree">The tree the node of the member is a part of.</param>
    /// <param name="node">The node the member is a part of.</param>
    /// <param name="member">The member to apply the transformation to.</param>
    /// <returns>A new member with the applied transformation,
    /// or <see langword="null"/> if the member should be removed.</returns>
    AstNodeMember? ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member);
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
    /// <param name="tree">The tree the node is a part of.</param>
    /// <param name="node">The node to check.</param>
    bool IsMatch(AstNodeHierarchy tree, AstNode node);
    /// <summary>
    /// Returns whether a member of a node matches the pattern.
    /// </summary>
    /// <param name="tree">The tree the member is a part of.</param>
    /// <param name="node">The node the member is a part of.</param>
    /// <param name="member">The node member to check.</param>
    bool IsMatch(AstNodeHierarchy tree, AstNode node, AstNodeMember member);
}
