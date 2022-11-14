using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Patterns;

/// <summary>
/// Contains extension and utility methods for transformation patterns.
/// </summary>
public static class Pattern
{
    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which matches a node or member based on its path. 
    /// </summary>
    /// <param name="path">The target path.</param>
    public static ITransformationPattern Path(NodePath path) =>
        new LambdaPattern()
        {
            MemberMatch = (_, node, member) =>
                node.GetPath().CreateLeafPath(member.Name) == path,

            NodeMatch = (_, node) =>
                node.GetPath() == path,
        };

    /// <summary>
    /// A pattern which always returns true.
    /// </summary>
    public static ITransformationPattern True { get; } = new LambdaPattern()
    {
        MemberMatch = (_, _, _) => true,
        NodeMatch = (_, _) => true,
        TreeMatch = _ => true
    };

    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which is triggered as a binary operation between two patterns.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <param name="op">The operation to apply between the patterns.</param>
    public static ITransformationPattern Binary(this ITransformationPattern left, ITransformationPattern right, Func<bool, bool, bool> op) =>
        new LambdaPattern()
        {
            MemberMatch = (tree, node, member) =>
                op(left.IsMatch(tree, node, member), right.IsMatch(tree, node, member)),

            NodeMatch = (tree, node) =>
                op(left.IsMatch(tree, node), right.IsMatch(tree, node)),

            TreeMatch = tree =>
                op(left.IsMatch(tree), right.IsMatch(tree))
        };

    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which is triggered only if two patterns both match.
    /// A logical and operation.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public static ITransformationPattern And(this ITransformationPattern left, ITransformationPattern right) =>
        Binary(left, right, (a, b) => a && b);

    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which is triggered if one or both of two patterns match.
    /// A logical or operation.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public static ITransformationPattern Or(this ITransformationPattern left, ITransformationPattern right) =>
        Binary(left, right, (a, b) => a || b);

    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which is triggered if only one of two patterns match.
    /// A logical exclusive-or operation.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public static ITransformationPattern Xor(this ITransformationPattern left, ITransformationPattern right) =>
        Binary(left, right, (a, b) => a ^ b);

    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which is triggered only if a pattern does not match.
    /// A logical not operation.
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static ITransformationPattern Not(this ITransformationPattern pattern) =>
        new LambdaPattern()
        {
            MemberMatch = (tree, node, member) =>
                !pattern.IsMatch(tree, node, member),

            NodeMatch = (tree, node) =>
                !pattern.IsMatch(tree, node),

            TreeMatch = tree =>
                !pattern.IsMatch(tree)
        };

    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which is triggered by a condition on all results of an enumerable of patterns.
    /// </summary>
    /// <param name="patterns">The patterns to match.</param>
    /// <param name="op">The condition to apply to the patterns.</param>
    public static ITransformationPattern Enumerable(this IEnumerable<ITransformationPattern> patterns, Func<IEnumerable<bool>, bool> op) =>
        new LambdaPattern()
        {
            MemberMatch = (tree, node, member) =>
                op(patterns.Select(p => p.IsMatch(tree, node, member))),

            NodeMatch = (tree, node) =>
                op(patterns.Select(p => p.IsMatch(tree, node))),

            TreeMatch = tree => 
                op(patterns.Select(p => p.IsMatch(tree)))
        };

    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which is triggered if all patterns of an enumerable match.
    /// </summary>
    /// <param name="patterns">The patterns to match.</param>
    public static ITransformationPattern All(this IEnumerable<ITransformationPattern> patterns) =>
        Enumerable(patterns, xs => xs.All(x => x));

    /// <summary>
    /// Returns an <see cref="ITransformationPattern"/>
    /// which is triggered if any pattern of an enumerable of patterns match.
    /// </summary>
    /// <param name="patterns">The patterns to match.</param>
    public static ITransformationPattern Any(this IEnumerable<ITransformationPattern> patterns) =>
        Enumerable(patterns, xs => xs.Any(x => x));
}
