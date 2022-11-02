namespace NanopassSharp.Transformations;

public static class Transform
{
    public static ITransformation AddChild(AstNode child) =>
        new LambdaBuilderTransformation()
        {
            NodeTransformer = (_, builder) =>
                builder.AddChild(child)
        };

    public static ITransformation AddMember(AstNodeMember member) =>
        new LambdaBuilderTransformation()
        {
            NodeTransformer = (_, builder) =>
                builder.AddMember(member)
        };

    public static ITransformation Remove { get; } =
        new LambdaTransformation()
        {
            MemberTransformer = (_, _, _) =>
                null,

            NodeTransformer = (_, _) =>
                null
        };

    public static ITransformation ReplaceTree(AstNodeHierarchy tree) =>
        new LambdaTransformation()
        {
            TreeTransformer = _ =>
                tree
        };

    public static ITransformation ReplaceNode(AstNode node) =>
        new LambdaTransformation()
        {
            NodeTransformer = (_, _) =>
                node
        };

    public static ITransformation ReplaceMember(AstNodeMember member) =>
        new LambdaTransformation()
        {
            MemberTransformer = (_, _, _) =>
                member
        };
}
