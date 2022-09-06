using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for an <see cref="AstNodeHierarchy"/>.
/// </summary>
public sealed class AstNodeHierarchyBuilder
{
    private readonly Dictionary<NodePath, AstNodeBuilder> builders;

    /// <summary>
    /// The roots of the hierarchy.
    /// </summary>
    public ICollection<string> Roots { get; set; }



    /// <summary>
    /// Initializes a new <see cref="AstNodeHierarchyBuilder"/> instance.
    /// </summary>
    public AstNodeHierarchyBuilder()
    {
        builders = new();
        Roots = new List<string>();
    }



    /// <summary>
    /// Creates a new <see cref="AstNodeHierarchyBuilder"/>
    /// from a <see cref="AstNodeHierarchy"/>.
    /// </summary>
    /// <param name="hierarchy">The source hierarchy.</param>
    public static AstNodeHierarchyBuilder FromHierarchy(AstNodeHierarchy hierarchy)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new node in the hierarchy.
    /// </summary>
    /// <param name="name">The name of the new node.</param>
    /// <param name="parentPath">The path to the parent of the node.
    /// If <see langword="null"/> then the node will be a root node.</param>
    /// <returns>A builder for the new node.</returns>
    /// <remarks>
    /// If a builder for a node with the same path already exists,
    /// then the already existing builder will be returned.
    /// </remarks>
    public AstNodeBuilder CreateNode(string name, string? parentPath)
    {
        NodePath? pp = parentPath is null
            ? null
            : NodePath.ParseUnsafe(parentPath);
        return CreateNode(name, pp);
    }
    /// <summary>
    /// Creates a new node in the hierarchy.
    /// </summary>
    /// <param name="name">The name of the new node.</param>
    /// <param name="parentPath">The path to the parent of the node.
    /// If <see langword="null"/> then the node will be a root node.</param>
    /// <returns>A builder for the new node.</returns>
    /// <remarks>
    /// If a builder for a node with the same path already exists,
    /// then the already existing builder will be returned.
    /// </remarks>
    public AstNodeBuilder CreateNode(string name, NodePath? parentPath) =>
        CreateNode(parentPath?.CreateLeafPath(name) ?? new(name));
    /// <summary>
    /// Creates a new node in the hierarchy.
    /// </summary>
    /// <param name="fullPath">The full path to the node.</param>
    /// <returns>A builder for the new node.</returns>
    /// <remarks>
    /// If a builder for a node with the same path already exists,
    /// then the already existing builder will be returned.
    /// </remarks>
    public AstNodeBuilder CreateNode(string fullPath) =>
        CreateNode(NodePath.ParseUnsafe(fullPath));
    /// <summary>
    /// Creates a new node in the hierarchy.
    /// </summary>
    /// <param name="fullPath">The full path to the node.</param>
    /// <returns>A builder for the new node.</returns>
    /// <remarks>
    /// If a builder for a node with the same path already exists,
    /// then the already existing builder will be returned.
    /// </remarks>
    public AstNodeBuilder CreateNode(NodePath fullPath)
    {
        if (builders.TryGetValue(fullPath, out var b)) return b;

        AstNodeBuilder builder = new(this, fullPath);
        builders.Add(fullPath, builder);

        return builder;
    }
    /// <summary>
    /// Creates a new node in the hierarchy from an existing <see cref="AstNode"/>.
    /// </summary>
    /// <param name="node">The node to create the new node from.</param>
    /// <returns>A builder for the new node.</returns>
    /// <remarks>
    /// If a builder for a node with the same path already exists,
    /// then the already existing builder will be overwritten with the data from the node.
    /// </remarks>
    public AstNodeBuilder CreateNode(AstNode node)
    {
        var path = node.GetPath();
        
        var builder = CreateNode(path);
        builder.Documentation = node.Documentation;
        builder.Children = node.Children.Keys.ToList();
        builder.Members = node.Members.Keys.ToList();
        builder.Attributes = new HashSet<object>(node.Attributes);

        return builder;
    }

    /// <summary>
    /// Adds a root node to the hierarchy.
    /// </summary>
    /// <param name="name">The name of the new node.</param>
    /// <returns>A builder for the new node.</returns>
    /// <remarks>
    /// If a builder for a root with the same name already exists,
    /// then the already existing builder will be returned.
    /// </remarks>
    public AstNodeBuilder AddRoot(string name)
    {
        var builder = CreateNode(name);
        Roots.Add(name);

        return builder;
    }

    /// <summary>
    /// Builds an <see cref="AstNodeHierarchy"/> from the builder.
    /// </summary>
    public AstNodeHierarchy Build()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Implicitly converts an <see cref="AstNodeBuilder"/> to an <see cref="AstNodeHierarchy"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator AstNodeHierarchy(AstNodeHierarchyBuilder builder) =>
        builder.Build();
}
