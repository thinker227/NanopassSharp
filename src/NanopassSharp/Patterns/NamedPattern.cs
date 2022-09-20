using System;

namespace NanopassSharp.Patterns;

/// <summary>
/// A pattern which matches a node or member based on its name.
/// </summary>
public sealed class NamePattern : ITransformationPattern
{
    /// <summary>
    /// The target for the pattern.
    /// </summary>
    public NamePatternTarget Target { get; }

    /// <summary>
    /// The target name.
    /// </summary>
    public string Name { get; }

    public bool IsRecursive => false;



    /// <summary>
    /// Initializes a new <see cref="NamePattern"/> instance.
    /// </summary>
    /// <param name="target"><inheritdoc cref="Target" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    public NamePattern(NamePatternTarget target, string name)
    {
        Target = target;
        Name = name;
    }



    /// <inheritdoc/>
    /// <remarks>
    /// Will always return <see langword="false"/>.
    /// </remarks>
    public bool IsMatch(AstNodeHierarchy tree) => false;

    public bool IsMatch(AstNodeHierarchy tree, AstNode node) =>
        Target.HasFlag(NamePatternTarget.Node) && node.Name == Name;

    public bool IsMatch(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        Target.HasFlag(NamePatternTarget.Member) && member.Name == Name;
}

/// <summary>
/// A target for a <see cref="NamePattern"/>.
/// </summary>
[Flags]
public enum NamePatternTarget
{
    None = 0,
    /// <summary>
    /// The pattern will match nodes.
    /// </summary>
    Node = 1,
    /// <summary>
    /// The pattern will match members.
    /// </summary>
    Member = 2,
    /// <summary>
    /// The pattern will match both nodes and members.
    /// </summary>
    Both = Node | Member
}
