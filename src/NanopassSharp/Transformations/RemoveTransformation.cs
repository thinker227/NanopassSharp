namespace NanopassSharp.Transformations;

/// <summary>
/// A transformation which removes nodes and members.
/// </summary>
public sealed class RemoveTransformation : BaseTransformation
{
    public override AstNode? ApplyToNode(AstNodeHierarchy tree, AstNode node) =>
        null;

    public override AstNodeMember? ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        null;
}
