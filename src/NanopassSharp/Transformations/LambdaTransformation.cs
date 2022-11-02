using System;

namespace NanopassSharp.Transformations;

public sealed class LambdaTransformation : ITransformation
{
    public Func<AstNodeHierarchy, AstNode, AstNodeMember, AstNodeMember?> MemberTransformer { get; init; } =
        (_, _, member) => member;

    public Func<AstNodeHierarchy, AstNode, AstNode?> NodeTransformer { get; init; } =
        (_, node) => node;

    public Func<AstNodeHierarchy, AstNodeHierarchy> TreeTransformer { get; init; } =
        hierarchy => hierarchy;



    public AstNodeMember? ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        MemberTransformer(tree, node, member);

    public AstNode? ApplyToNode(AstNodeHierarchy tree, AstNode node) =>
        NodeTransformer(tree, node);

    public AstNodeHierarchy ApplyToTree(AstNodeHierarchy tree) =>
        TreeTransformer(tree);
}
