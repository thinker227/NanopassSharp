using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Patterns;

public sealed class EnumerablePattern : ITransformationPattern
{
    public bool IsRecursive { get; }
    public IEnumerable<ITransformationPattern> Patterns { get; }
    public Func<IEnumerable<bool>, bool> Func { get; }



    public EnumerablePattern(IEnumerable<ITransformationPattern> patterns, Func<IEnumerable<bool>, bool> func)
    {
        Patterns = patterns;
        Func = func;
    }



    public bool IsMatch(AstNodeHierarchy tree) =>
        Func(Patterns.Select(p => p.IsMatch(tree)));

    public bool IsMatch(AstNodeHierarchy tree, AstNode node) =>
        Func(Patterns.Select(p => p.IsMatch(tree, node)));

    public bool IsMatch(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        Func(Patterns.Select(p => p.IsMatch(tree, node, member)));
}
