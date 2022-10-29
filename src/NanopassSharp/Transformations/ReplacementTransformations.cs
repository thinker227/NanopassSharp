namespace NanopassSharp.Transformations;

/// <summary>
/// A transformation which replaces a hierarchy with a new hierarchy.
/// </summary>
public sealed class ReplaceHierarchyTransformation : BaseTransformation
{
    /// <summary>
    /// The hierarchy used to override.
    /// </summary>
    public AstNodeHierarchy Hierarchy { get; }

    /// <summary>
    /// Initializes a new <see cref="ReplaceHierarchyTransformation"/> instance.
    /// </summary>
    /// <param name="hierarchy"><inheritdoc cref="Hierarchy" path="/summary"/></param>
    public ReplaceHierarchyTransformation(AstNodeHierarchy hierarchy)
    {
        Hierarchy = hierarchy;
    }

    public override AstNodeHierarchy ApplyToTree(AstNodeHierarchy tree) =>
        Hierarchy;
}

/// <summary>
/// A transformation which replaces a node with a new node.
/// </summary>
public sealed class ReplaceNodeTransformation : BaseTransformation
{
    /// <summary>
    /// The node used to override.
    /// </summary>
    public AstNode? Node { get; }

    /// <summary>
    /// Initializes a new <see cref="ReplaceNodeTransformation"/> instance.
    /// </summary>
    /// <param name="node"><inheritdoc cref="Node" path="/summary"/></param>
    public ReplaceNodeTransformation(AstNode? node)
    {
        Node = node;
    }

    public override AstNode? ApplyToNode(AstNodeHierarchy tree, AstNode node) =>
        Node;
}

/// <summary>
/// A transformation which replaces a member with a new member.
/// </summary>
public sealed class ReplaceMemberTransformation : BaseTransformation
{
    /// <summary>
    /// The member used to override.
    /// </summary>
    public AstNodeMember? Member { get; }

    /// <summary>
    /// Initializes a new <see cref="ReplaceMemberTransformation"/> instance.
    /// </summary>
    /// <param name="member"><inheritdoc cref="Member" path="/summary"/></param>
    public ReplaceMemberTransformation(AstNodeMember? member)
    {
        Member = member;
    }

    public override AstNodeMember? ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        Member;
}
