using NanopassSharp.Builders;

namespace NanopassSharp.Transformations;

/// <summary>
/// A transformation which adds a child to a node.
/// </summary>
public sealed class AddNodeTransformation : BaseBuilderTransformation
{
    /// <summary>
    /// A node to add as a child.
    /// </summary>
    public AstNode Node { get; }

    /// <summary>
    /// Initializes a new <see cref="AddNodeTransformation"/> instance.
    /// </summary>
    /// <param name="node"><inheritdoc cref="Node" path="/summary"/></param>
    public AddNodeTransformation(AstNode node)
    {
        Node = node;
    }

    public override void ApplyToNode(AstNodeHierarchy tree, AstNodeBuilder builder) =>
        builder.AddChild(Node);
}

/// <summary>
/// A transformation which adds a member to a node.
/// </summary>
public sealed class AddMemberTransformation : BaseBuilderTransformation
{
    /// <summary>
    /// The member to add.
    /// </summary>
    public AstNodeMember Member { get; }

    /// <summary>
    /// Initializes a new <see cref="AddMemberTransformation"/> instance.
    /// </summary>
    /// <param name="member"><inheritdoc cref="Member" path="/summary"/></param>
    public AddMemberTransformation(AstNodeMember member)
    {
        Member = member;
    }

    public override void ApplyToNode(AstNodeHierarchy tree, AstNodeBuilder builder) =>
        builder.AddMember(Member);
}
