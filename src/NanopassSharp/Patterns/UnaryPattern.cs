using System;

namespace NanopassSharp.Patterns;

/// <summary>
/// A pattern which applies a unary operation to another pattern.
/// </summary>
public sealed class UnaryPattern : ITransformationPattern
{
    /// <summary>
    /// The operand pattern.
    /// </summary>
    public ITransformationPattern Operand { get; }

    /// <summary>
    /// The unary operation.
    /// </summary>
    public Func<bool, bool> Operation { get; }

    public bool IsRecursive =>
        Operand.IsRecursive;



    /// <summary>
    /// Initializes a new <see cref="UnaryPattern"/> instance.
    /// </summary>
    /// <param name="operand"><inheritdoc cref="Operand" path="/summary"/></param>
    /// <param name="operation"><inheritdoc cref="Operation" path="/summary"/></param>
    public UnaryPattern(ITransformationPattern operand, Func<bool, bool> operation)
    {
        Operand = operand;
        Operation = operation;
    }



    public bool IsMatch(AstNodeHierarchy tree) =>
        Operation(Operand.IsMatch(tree));
    public bool IsMatch(AstNodeHierarchy tree, AstNode node) =>
        Operation(Operand.IsMatch(tree, node));
    public bool IsMatch(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        Operation(Operand.IsMatch(tree, node, member));
}
