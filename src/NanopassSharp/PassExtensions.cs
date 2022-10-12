﻿using System;
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
    public static NodePath GetPath(this AstNode node)
    {
        List<string> pathNodes = new();

        for (var current = node; current is not null; current = current.Parent)
        {
            pathNodes.Insert(0, current.Name);
        }

        return new NodePath(pathNodes);
    }

    /// <summary>
    /// Gets a node in a <see cref="AstNodeHierarchy"/> from a path.
    /// </summary>
    /// <param name="hierarchy">The source hierarchy.</param>
    /// <param name="path">The path to get the node from.</param>
    /// <returns>The node with the specified path,
    /// or <see langword="null"/> if the node does not exist.</returns>
    public static AstNode? GetNodeFromPath(this AstNodeHierarchy hierarchy, NodePath path)
    {
        var root = hierarchy.Roots.FirstOrDefault(n => n.Name == path.Root);
        if (root is null) return null;

        return root.GetDecendantNodeFromPath(path, true);
    }

    /// <summary>
    /// Gets a decendant node of a node from a path.
    /// </summary>
    /// <param name="node">The node to get the decendant node of.</param>
    /// <param name="path">The path of the decendant node to get.</param>
    /// <param name="selfAsRoot">Whether the current node should be counted as the root of the path.</param>
    /// <returns>The decendant node at <paramref name="path"/>,
    /// or <see langword="null"/> if the node does not exist.</returns>
    public static AstNode? GetDecendantNodeFromPath(this AstNode node, NodePath path, bool selfAsRoot = false)
    {
        var nodesEnumerator = path.GetNodes().GetEnumerator();

        if (selfAsRoot)
        {
            nodesEnumerator.MoveNext();
            if (nodesEnumerator.Current != node.Name) return null;
        }

        var currentNode = node;
        while (nodesEnumerator.MoveNext())
        {
            string nodeName = nodesEnumerator.Current;
            if (!currentNode.Children.TryGetValue(nodeName, out var child)) return null;
            currentNode = child;
        }

        return currentNode;
    }
}
