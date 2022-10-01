using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp;

public static class PassExtensions
{
    public static IEnumerable<AstNode> GetNodes(this AstNodeHierarchy tree) =>
        tree.Roots.SelectMany(r => r.GetDecendantNodesAndSelf());

    public static IEnumerable<AstNode> GetDecendantNodes(this AstNode node) =>
        node.Children.Values.SelectMany(n => n.GetDecendantNodes());

    public static IEnumerable<AstNode> GetDecendantNodesAndSelf(this AstNode node) =>
        node.GetDecendantNodes().Prepend(node);

    public static AstNode GetRoot(this AstNode node) =>
        node.Parent?.GetRoot() ?? node;

    /// <summary>
    /// Gets the path to an <see cref="AstNode"/> from its root.
    /// </summary>
    /// <param name="node">The node to get the path to.</param>
    public static NodePath GetPath(this AstNode node) =>
        NodePath.Create(node, n => (n.Parent, n.Parent is not null), n => n.Name);

    public static AstNode? GetNodeFromPath(this AstNodeHierarchy hierarchy, NodePath path)
    {
        var nodesEnumerator = path.GetNodes().Reverse().GetEnumerator();

        nodesEnumerator.MoveNext();
        string rootName = nodesEnumerator.Current;
        var root = hierarchy.Roots.FirstOrDefault(r => r.Name == rootName);
        if (root is null) return null;

        var currentNode = root;
        while (nodesEnumerator.MoveNext())
        {
            string nodeName = nodesEnumerator.Current;
            if (!currentNode.Children.TryGetValue(nodeName, out var child)) return null;
            currentNode = child;
        }

        return currentNode;
    }
}
