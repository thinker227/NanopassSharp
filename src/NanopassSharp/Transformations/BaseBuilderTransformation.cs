using NanopassSharp.Builders;

namespace NanopassSharp.Transformations;

/// <summary>
/// A base implementation of <see cref="ITransformation"/> which exposes a builder.
/// </summary>
public abstract class BaseBuilderTransformation : ITransformation
{
    /// <inheritdoc cref="ITransformation.ApplyToMember(AstNodeHierarchy, AstNode, AstNodeMember)"/>
    /// <param name="builder">The builder for the member.</param>
    public virtual void ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMemberBuilder builder) { }

    /// <inheritdoc cref="ITransformation.ApplyToNode(AstNodeHierarchy, AstNode)"/>
    /// <param name="builder">The builder for the node.</param>
    public virtual void ApplyToNode(AstNodeHierarchy tree, AstNodeBuilder builder) { }

    /// <inheritdoc cref="ITransformation.ApplyToTree(AstNodeHierarchy)"/>
    /// <param name="builder">The builder for the tree.</param>
    public virtual void ApplyToTree(AstNodeHierarchyBuilder builder) { }

    public AstNodeMember? ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member)
    {
        var builder = AstNodeMemberBuilder.FromMember(member);
        ApplyToMember(tree, node, builder);
        return builder.Build();
    }

    public AstNode? ApplyToNode(AstNodeHierarchy tree, AstNode node)
    {
        var builder = AstNodeHierarchyBuilder.FromHierarchy(tree).CreateNode(node);
        ApplyToNode(tree, builder);
        return builder.Build();
    }

    public AstNodeHierarchy ApplyToTree(AstNodeHierarchy tree)
    {
        var builder = AstNodeHierarchyBuilder.FromHierarchy(tree);
        ApplyToTree(builder);
        return builder.Build();
    }
}
