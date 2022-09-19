namespace NanopassSharp.Transformations;

/// <summary>
/// A base implementation of <see cref="ITransformation"/>.
/// </summary>
public abstract class BaseTransformation : ITransformation
{
    /// <inheritdoc/>
    /// <remarks>
    /// Unless overwritten, returns <paramref name="member"/>.
    /// </remarks>
    public virtual AstNodeMember ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member) => member;
    /// <inheritdoc/>
    /// <remarks>
    /// Unless overwritten, returns <paramref name="node"/>.
    /// </remarks>
    public virtual AstNode ApplyToNode(AstNodeHierarchy tree, AstNode node) => node;
    /// <inheritdoc/>
    /// <remarks>
    /// Unless overwritten, returns <paramref name="tree"/>.
    /// </remarks>
    public virtual AstNodeHierarchy ApplyToTree(AstNodeHierarchy tree) => tree;
}
