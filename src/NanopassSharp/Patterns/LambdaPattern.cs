using System;

namespace NanopassSharp.Patterns;

public sealed class LambdaPattern : ITransformationPattern
{
    public bool IsRecursive => false;

    public Func<AstNodeHierarchy, bool> TreeMatch { get; init; } =
        _ => false;

    public Func<AstNodeHierarchy, AstNode, bool> NodeMatch { get; init; } =
        (_, _) => false;

    public Func<AstNodeHierarchy, AstNode, AstNodeMember, bool> MemberMatch { get; init; } =
        (_, _, _) => false;



    public bool IsMatch(AstNodeHierarchy tree) =>
        TreeMatch(tree);

    public bool IsMatch(AstNodeHierarchy tree, AstNode node) =>
        NodeMatch(tree, node);

    public bool IsMatch(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        MemberMatch(tree, node, member);
}
