namespace NanopassSharp;

public static class TransformationExtensions
{
    /// <summary>
    /// Applies a transformation to a tree if it matches the transformation pattern.
    /// </summary>
    /// <param name="description">The source transformation description.</param>
    /// <param name="tree">The tree to apply the transformation to.</param>
    /// <returns>A new tree transformed by the transformation if the tree
    /// matches the transformation pattern, otherwise the original tree.</returns>
    public static AstNodeHierarchy Apply(this ITransformationDescription description, AstNodeHierarchy tree) =>
        description.Pattern?.IsMatch(tree) ?? true
            ? description.Transformation.ApplyToTree(tree)
            : tree;
    /// <summary>
    /// Applies a transformation to a node if it matches the transformation pattern.
    /// </summary>
    /// <param name="description">The source transformation description.</param>
    /// <param name="tree">The tree the node is a part of.</param>
    /// <param name="node">The node to apply the transformation to.</param>
    /// <returns>A new tree transformed by the transformation if the tree
    /// matches the transformation pattern, otherwise the original tree.</returns>
    public static AstNodeHierarchy Apply(this ITransformationDescription description, AstNodeHierarchy tree, AstNode node) =>
        description.Pattern?.IsMatch(node) ?? true
            ? description.Transformation.ApplyToNode(tree, node)
            : tree;
    /// <summary>
    /// Applies a transformation to a node member if it matches the transformation pattern.
    /// </summary>
    /// <param name="description">The source transformation description.</param>
    /// <param name="tree">The tree the node is a part of.</param>
    /// <param name="node">The node to apply the transformation to.</param>
    /// <param name="member">The member to apply the transformation to.</param>
    /// <returns>A new tree transformed by the transformation if the tree
    /// matches the transformation pattern, otherwise the original tree.</returns>
    public static AstNodeHierarchy Apply(this ITransformationDescription description, AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        description.Pattern?.IsMatch(node, member) ?? true
            ? description.Transformation.ApplyToMember(tree, node, member)
            : tree;
}
