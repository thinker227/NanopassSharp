using System;
using NanopassSharp.Builders;

namespace NanopassSharp.Transformations;

public sealed class LambdaBuilderTransformation : ITransformation
{
    public Action<AstNodeHierarchy, AstNode, AstNodeMemberBuilder> MemberTransformer { get; init; } =
        (_, _, _) => { };

    public Action<AstNodeHierarchy, AstNodeBuilder> NodeTransformer { get; init; } =
        (_, _) => { };

    public Action<AstNodeHierarchyBuilder> TreeTransformer { get; init; } =
        _ => { };



    public AstNodeMember? ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member)
    {
        var builder = AstNodeMemberBuilder.FromMember(member);
        MemberTransformer(tree, node, builder);
        return builder.Build();
    }

    public AstNode? ApplyToNode(AstNodeHierarchy tree, AstNode node)
    {
        var builder = new AstNodeHierarchyBuilder().CreateNode(node);
        NodeTransformer(tree, builder);
        return builder.Build();
    }

    public AstNodeHierarchy ApplyToTree(AstNodeHierarchy tree)
    {
        var builder = AstNodeHierarchyBuilder.FromHierarchy(tree);
        TreeTransformer(builder);
        return builder.Build();
    }
}
