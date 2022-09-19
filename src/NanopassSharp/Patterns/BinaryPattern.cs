using System;

namespace NanopassSharp.Patterns;

/// <summary>
/// A pattern which applies a binary operation to two other patterns.
/// </summary>
public sealed class BinaryPattern : ITransformationPattern
{
    /// <summary>
    /// The left pattern.
    /// </summary>
    public ITransformationPattern Left { get; }

    /// <summary>
    /// The right pattern.
    /// </summary>
    public ITransformationPattern Right { get; }

    /// <summary>
    /// The binary operation.
    /// </summary>
    public Func<bool, bool, bool> Operation { get; }

    public bool IsRecursive =>
        Left.IsRecursive || Right.IsRecursive;



    /// <summary>
    /// Initializes a new <see cref="BinaryPattern"/> instance.
    /// </summary>
    /// <param name="left"><inheritdoc cref="Left" path="/summary"/></param>
    /// <param name="right"><inheritdoc cref="Right" path="/summary"/></param>
    /// <param name="operation"><inheritdoc cref="Operation" path="/summary"/></param>
    public BinaryPattern(ITransformationPattern left, ITransformationPattern right, Func<bool, bool, bool> operation)
    {
        Left = left;
        Right = right;
        Operation = operation;
    }



    public bool IsMatch(AstNodeHierarchy tree) =>
        Operation(Left.IsMatch(tree), Right.IsMatch(tree));

    public bool IsMatch(AstNodeHierarchy tree, AstNode node) =>
        Operation(Left.IsMatch(tree, node), Right.IsMatch(tree, node));

    public bool IsMatch(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        Operation(Left.IsMatch(tree, node, member), Right.IsMatch(tree, node, member));
}
