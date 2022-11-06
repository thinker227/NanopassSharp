namespace NanopassSharp.Transformations;

/// <summary>
/// Contains utility methods for transformations.
/// </summary>
public static class Transform
{
    /// <summary>
    /// Returns an <see cref="ITransformation"/>
    /// which adds a child to an <see cref="AstNode"/>.
    /// </summary>
    /// <param name="child">The child to add.</param>
    public static ITransformation AddChild(AstNode child) =>
        new LambdaBuilderTransformation()
        {
            NodeTransformer = (_, builder) =>
                builder.AddChild(child)
        };

    /// <summary>
    /// Returns an <see cref="ITransformation"/>
    /// which adds a member to an <see cref="AstNode"/>.
    /// </summary>
    /// <param name="member">The member to add.</param>
    public static ITransformation AddMember(AstNodeMember member) =>
        new LambdaBuilderTransformation()
        {
            NodeTransformer = (_, builder) =>
                builder.AddMember(member)
        };

    /// <summary>
    /// An <see cref="ITransformation"/>
    /// which removes a member or node.
    /// </summary>
    public static ITransformation Remove { get; } =
        new LambdaTransformation()
        {
            MemberTransformer = (_, _, _) =>
                null,

            NodeTransformer = (_, _) =>
                null
        };

    /// <summary>
    /// Returns an <see cref="ITransformation"/>
    /// which replaces a tree with another tree.
    /// </summary>
    /// <param name="tree">The tree to replace with.</param>
    public static ITransformation ReplaceTree(AstNodeHierarchy tree) =>
        new LambdaTransformation()
        {
            TreeTransformer = _ =>
                tree
        };

    /// <summary>
    /// Returns an <see cref="ITransformation"/>
    /// which replaces a node with another node.
    /// </summary>
    /// <param name="node">The node to replace with.</param>
    public static ITransformation ReplaceNode(AstNode node) =>
        new LambdaTransformation()
        {
            NodeTransformer = (_, _) =>
                node
        };

    /// <summary>
    /// Returns an <see cref="ITransformation"/>
    /// which replaces a member with another member.
    /// </summary>
    /// <param name="member">The member to replace with.</param>
    public static ITransformation ReplaceMember(AstNodeMember member) =>
        new LambdaTransformation()
        {
            MemberTransformer = (_, _, _) =>
                member
        };
}
