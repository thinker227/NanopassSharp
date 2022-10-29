namespace NanopassSharp.Patterns;

/// <summary>
/// A pattern which matches a node or member based on its path.
/// </summary>
public sealed class PathPattern : ITransformationPattern
{
    /// <summary>
    /// The target path.
    /// </summary>
    public NodePath Path { get; }

    public bool IsRecursive => false;



    /// <summary>
    /// Initializes a new <see cref="PathPattern"/> instance.
    /// </summary>
    /// <param name="path"><inheritdoc cref="Path" path="/summary"/></param>
    public PathPattern(NodePath path)
    {
        Path = path;
    }

    /// <summary>
    /// Initializes a new <see cref="PathPattern"/> instance.
    /// </summary>
    /// <param name="path"><inheritdoc cref="Path" path="/summary"/></param>
    public PathPattern(string path) : this(NodePath.ParseUnsafe(path)) { }



    /// <inheritdoc/>
    /// <remarks>
    /// Will always return <see langword="false"/>.
    /// </remarks>
    public bool IsMatch(AstNodeHierarchy tree) => false;

    public bool IsMatch(AstNodeHierarchy tree, AstNode node) =>
        node.GetPath() == Path;

    public bool IsMatch(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        node.GetPath().CreateLeafPath(member.Name) == Path;
}
