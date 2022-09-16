using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NanopassSharp;

/// <summary>
/// A path to a node in a node graph.
/// </summary>
public readonly struct NodePath : IEquatable<NodePath>
{
    private readonly string[] nodes;

    private bool IsEmpty => nodes.Length <= 0;

    /// <summary>
    /// Gets the path to the parent node of the current outer-most leaf node.
    /// </summary>
    public NodePath Parent
    {
        get
        {
            if (IsEmpty)
            {
                throw NoNodes();
            }
            if (IsRoot)
            {
                throw new InvalidOperationException("Cannot get the parent of the root");
            }

            return new(nodes[..^1]);
        }
    }

    /// <summary>
    /// The name of the root node of the graph.
    /// </summary>
    public string Root
    {
        get
        {
            if (IsEmpty)
            {
                throw NoNodes();
            }

            return nodes[0];
        }
    }

    /// <summary>
    /// The name of the node the path leads to, the outer "leaf" of the node graph.
    /// </summary>
    public string Leaf => !IsEmpty
        ? nodes[^1]
        : throw NoNodes();

    /// <summary>
    /// The depth of the node the path leads to. 0 for the root node, 1 for the root's children, etc.
    /// </summary>
    public int Depth => nodes.Length >= 1
        ? nodes.Length - 1
        : throw NoNodes();

    /// <summary>
    /// Whether the path is only a root node.
    /// </summary>
    public bool IsRoot => Depth == 0;



    /// <summary>
    /// Initializes a new <see cref="NodePath"/> instance.
    /// </summary>
    /// <remarks>
    /// The initialized instance will always produce <see cref="InvalidOperationException"/>
    /// due to being in an invalid state.
    /// </remarks>
    public NodePath()
    {
        // This is just to avoid non-descript NREs.
        // If this ctor is called, the nodes will be empty and the
        // properties will throw appropriate exceptions when accessed.
        nodes = Array.Empty<string>();
    }

    /// <summary>
    /// Initializes a new <see cref="NodePath"/> instance.
    /// </summary>
    /// <param name="nodes">The nodes the path consists of,
    /// in order from the root to the outer-most leaf.</param>
    public NodePath(IEnumerable<string> nodes)
    {
        this.nodes = nodes.ToArray();

        if (this.nodes.Length <= 0)
        {
            throw new ArgumentException("Length of nodes has to be greater or equal to 1.", nameof(nodes));
        }
    }

    /// <summary>
    /// Initializes a new <see cref="NodePath"/> instance.
    /// </summary>
    /// <param name="nodes">The nodes the path consists of,
    /// in order from the root to the outer-most leaf.</param>
    public NodePath(params string[] nodes) : this((IEnumerable<string>)nodes) { }

    /// <summary>
    /// Initializes a new <see cref="NodePath"/> instance.
    /// </summary>
    /// <param name="root">The root node of the path.</param>
    public NodePath(string root)
    {
        nodes = new[] { root };
    }



    private void ThrowIfNoNodes()
    {
        if (IsEmpty) throw NoNodes();
    }

    private static InvalidOperationException NoNodes() =>
        new("The path contains no nodes");

    public bool Equals(NodePath other)
    {
        if (other.nodes.Length != nodes.Length) return false;

        for (int i = 0; i < nodes.Length; i++)
        {
            if (other.nodes[i] != nodes[i]) return false;
        }

        return true;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is NodePath nodePath && Equals(nodePath);

    public override int GetHashCode()
    {
        HashCode hashCode = new();
        foreach (string node in nodes)
        {
            hashCode.Add(node);
        }
        return hashCode.ToHashCode();
    }

    public override string ToString() =>
        string.Join('.', nodes);

    /// <summary>
    /// Parses a <see cref="NodePath"/> from a string,
    /// separating node names a period character ('.').
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The parsed <see cref="NodePath"/>,
    /// or <see langword="null"/> if the input string was in an incorrect format.</returns>
    public static NodePath? Parse(string s)
    {
        List<string> nodes = new();

        int currentStart = 0;
        for (int i = 0; i <= s.Length; i++)
        {
            if (i == s.Length || s[i] == '.')
            {
                if (currentStart == i)
                {
                    return null;
                }

                string node = s[currentStart..i];
                nodes.Add(node);

                currentStart = i + 1;
            }
        }

        return new NodePath(nodes);
    }

    /// <summary>
    /// Parses a <see cref="NodePath"/> from a string,
    /// separating node names a period character ('.').
    /// Throws an exception if the input string is not in the correct format.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The parsed <see cref="NodePath"/>.</returns>
    /// <exception cref="FormatException">
    /// <paramref name="s"/> is not in the correct format.
    /// </exception>
    public static NodePath ParseUnsafe(string s) =>
        Parse(s) ?? throw new FormatException("String was not in the correct format");

    /// <summary>
    /// Creates a new <see cref="NodePath"/> from a recursive structure.
    /// </summary>
    /// <typeparam name="T">The type of the recursive structure.</typeparam>
    /// <param name="start">The object which starts the recursive structure,
    /// the object which will create the outer-most leaf node of the path.</param>
    /// <param name="nextSelector">A function to select the next object of the
    /// recursive structure, and whether the object has a next.</param>
    /// <param name="nodeSelector">A function to select the node name from an object.</param>
    /// <returns>A new <see cref="NodePath"/> created by
    /// iterating through the recursive structure.</returns>
    public static NodePath Create<T>(T start, Func<T, (T?, bool)> nextSelector, Func<T, string> nodeSelector) where T : class
    {
        List<string> nodes = new();
        HashSet<T> visited = new(ReferenceEqualityComparer.Instance);

        var current = start;
        while (true)
        {
            if (visited.Contains(current))
            {
                throw new InvalidOperationException("Instance has already been visited");
            }
            visited.Add(current);

            string node = nodeSelector(current);
            nodes.Insert(0, node);

            var (next, hasNext) = nextSelector(current);
            if (!hasNext) break;
            current = next!;
        }

        return new NodePath(nodes);
    }

    /// <summary>
    /// Creates a new path to a leaf node.
    /// </summary>
    /// <param name="leafNode">The name of the leaf node.</param>
    /// <returns>A new <see cref="NodePath"/> to a leaf node with the
    /// name <paramref name="leafNode"/> and the current path as its parent.</returns>
    public NodePath CreateLeafPath(string leafNode) =>
        new(nodes.Append(leafNode));

    /// <summary>
    /// Gets the paths to all parent nodes of the current path,
    /// in order from the parent of the current outer-most leaf to the root node.
    /// </summary>
    public IEnumerable<NodePath> GetParentPaths()
    {
        ThrowIfNoNodes();

        if (IsRoot) yield break;

        for (int i = nodes.Length - 1; i > 0; i--)
        {
            yield return new NodePath(nodes[0..i]);
        }
    }

    /// <summary>
    /// Gets the paths to all parent nodes of the current path including the current path,
    /// in order from the current path to the root node.
    /// </summary>
    public IEnumerable<NodePath> GetParentPathsAndSelf() =>
        GetParentPaths().Prepend(this);

    /// <summary>
    /// Gets the nodes in the path, in order from the current leaf to the root.
    /// </summary>
    public IEnumerable<string> GetNodes() =>
        nodes.Reverse();

    public static bool operator ==(NodePath a, NodePath b) =>
        a.Equals(b);

    public static bool operator !=(NodePath a, NodePath b) =>
        !a.Equals(b);
}
