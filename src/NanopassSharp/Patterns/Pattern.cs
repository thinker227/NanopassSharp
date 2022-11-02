using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Patterns;

public static class Pattern
{
    public static ITransformationPattern Path(NodePath path) =>
        new LambdaPattern()
        {
            MemberMatch = (_, node, member) =>
                node.GetPath().CreateLeafPath(member.Name) == path,

            NodeMatch = (_, node) =>
                node.GetPath() == path,
        };

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

    public static ITransformationPattern And(this ITransformationPattern left, ITransformationPattern right) =>
        Binary(left, right, (a, b) => a && b);

    public static ITransformationPattern Or(this ITransformationPattern left, ITransformationPattern right) =>
        Binary(left, right, (a, b) => a || b);

    public static ITransformationPattern Xor(this ITransformationPattern left, ITransformationPattern right) =>
        Binary(left, right, (a, b) => a ^ b);

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

    public static ITransformationPattern All(this IEnumerable<ITransformationPattern> patterns) =>
        Enumerable(patterns, xs => xs.All(x => x));

    public static ITransformationPattern Any(this IEnumerable<ITransformationPattern> patterns) =>
        Enumerable(patterns, xs => xs.Any(x => x));
}
