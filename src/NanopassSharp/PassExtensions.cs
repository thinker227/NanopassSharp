using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp;

public static class PassExtensions
{
    /// <summary>
    /// Gets all nodes of a <see cref="AstNodeHierarchy"/>.
    /// </summary>
    /// <param name="tree">The hierarchy to get the nodes of.</param>
    /// <returns>A collection of nodes.</returns>
    public static IEnumerable<AstNode> GetNodes(this AstNodeHierarchy tree) =>
        tree.Roots.SelectMany(r => GetDecendantNodes(r, true));

    /// <summary>
    /// Gets the decendant nodes of a node, excluding itself.
    /// </summary>
    /// <param name="node">The node to get the decendants of.</param>
    /// <returns>A collection of decendant nodes.</returns>
    public static IEnumerable<AstNode> GetDecendantNodes(this AstNode node) =>
        GetDecendantNodes(node, false);

    /// <summary>
    /// Gets the decendant nodes of a node, including itself.
    /// </summary>
    /// <param name="node">The node to get the decendants of.</param>
    /// <returns>A collection of decendant nodes and the current node.</returns>
    public static IEnumerable<AstNode> GetDecendantNodesAndSelf(this AstNode node) =>
        GetDecendantNodes(node, true);

    private static IEnumerable<AstNode> GetDecendantNodes(AstNode node, bool includeSelf)
    {
        List<AstNode> nodes = new();

        if (includeSelf)
        {
            nodes.Add(node);
        }

        AddDecendantNodes(node, nodes);

        return nodes;
    }

    private static void AddDecendantNodes(AstNode node, List<AstNode> nodes)
    {
        foreach (var child in node.Children.Values)
        {
            nodes.Add(child);
            AddDecendantNodes(child, nodes);
        }
    }

    /// <summary>
    /// Gets the root of an <see cref="AstNode"/>.
    /// </summary>
    /// <param name="node">The node to get the root of.</param>
    public static AstNode GetRoot(this AstNode node) =>
        node.Parent?.GetRoot() ?? node;

    /// <summary>
    /// Gets the path to an <see cref="AstNode"/> from its root.
    /// </summary>
    /// <param name="node">The node to get the path to.</param>
    public static NodePath GetPath(this AstNode node) =>
        NodePath.Create(node, n => (n.Parent, n.Parent is not null), n => n.Name);

    /// <summary>
    /// Gets a node in a <see cref="AstNodeHierarchy"/> from a path.
    /// </summary>
    /// <param name="hierarchy">The source hierarchy.</param>
    /// <param name="path">The path to get the node from.</param>
    /// <returns>The node with the specified path,
    /// or <see langword="null"/> if the node does not exist.</returns>
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
