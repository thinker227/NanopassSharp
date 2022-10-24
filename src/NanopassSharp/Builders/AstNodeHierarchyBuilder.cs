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
        AstNodeHierarchyBuilder builder = new();

        foreach (var root in hierarchy.Roots)
        {
            AddNodeAndChildren(builder, root);
            builder.Roots = hierarchy.Roots
                .Select(n => n.Name)
                .ToArray();
        }

        return builder;
    }

    private static void AddNodeAndChildren(AstNodeHierarchyBuilder builder, AstNode node)
    {
        var nodeBuilder = builder.CreateNode(node)
            .WithDocumentation(node.Documentation)
            .WithAttributes(new HashSet<object>(node.Attributes));

        foreach (var child in node.Children.Values)
        {
            AddNodeAndChildren(builder, child);
        }
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
    /// <param name="behavior">The behavior for how to create the node.</param>
    /// <returns>A builder for the new node.</returns>
    /// <remarks>
    /// If a builder for a node with the same path already exists,
    /// then the already existing builder will be overwritten with the data from the node.
    /// </remarks>
    public AstNodeBuilder CreateNode(AstNode node, CreateNodeBehavior behavior = CreateNodeBehavior.CreateFromRoot)
    {
        var path = node.GetPath();

        if (behavior == CreateNodeBehavior.CreateFromRoot)
        {
            // Create the root and all children, then return the requested node
            var root = node.GetRoot();
            CreateNode(root, CreateNodeBehavior.CreateInPlaceWithChildren);
            return GetNodeFromPath(path)!;
        }

        var builder = CreateNode(path);
        builder.Documentation = node.Documentation;
        builder.Children = node.Children.Keys.ToList();
        builder.Attributes = new HashSet<object>(node.Attributes);

        if (behavior == CreateNodeBehavior.CreateInPlaceWithChildren)
        {
            foreach (var child in node.Children.Values)
            {
                CreateNode(child, behavior);
            }
        }

        foreach (var member in node.Members.Values)
        {
            builder.AddMember(member);
        }

        if (path.IsRoot)
        {
            AddRoot(path.Root);
        }

        return builder;
    }

    /// <summary>
    /// Removes a node from the hierarchy.
    /// </summary>
    /// <param name="fullPath">The full path of the node to remove.</param>
    /// <returns>The current builder.</returns>
    public AstNodeHierarchyBuilder RemoveNode(string fullPath) =>
        RemoveNode(NodePath.ParseUnsafe(fullPath));

    /// <summary>
    /// Removes a node from the hierarchy.
    /// </summary>
    /// <param name="fullPath">The full path of the node to remove.</param>
    /// <returns>The current builder.</returns>
    public AstNodeHierarchyBuilder RemoveNode(NodePath fullPath)
    {
        builders.Remove(fullPath);

        if (fullPath.IsRoot)
        {
            Roots.Remove(fullPath.Leaf);
        }

        return this;
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
    /// Gets a <see cref="AstNodeBuilder"/> from a specified path to a node.
    /// </summary>
    /// <param name="path">The path to get the builder from.</param>
    /// <returns>The builder for the node that <paramref name="path"/> specified,
    /// or <see langword="null"/> if there is no builder for the path.</returns>
    public AstNodeBuilder? GetNodeFromPath(NodePath path) =>
        builders.TryGetValue(path, out var builder)
            ? builder
            : null;

    /// <summary>
    /// Gets a <see cref="AstNodeBuilder"/> from a specified path to a node.
    /// </summary>
    /// <param name="path">The path to get the builder from.</param>
    /// <returns>The builder for the node that <paramref name="path"/> specified,
    /// or <see langword="null"/> if there is no builder for the path.</returns>
    public AstNodeBuilder? GetNodeFromPath(string path) =>
        GetNodeFromPath(NodePath.ParseUnsafe(path));

    /// <summary>
    /// Builds an <see cref="AstNodeHierarchy"/> from the builder.
    /// </summary>
    /// <param name="missingChildBehavior">The behavior if a node is missing.</param>
    public AstNodeHierarchy Build(MissingChildBehavior missingChildBehavior = MissingChildBehavior.Throw)
    {
        List<AstNode> roots = new();
        AstNodeHierarchy hierarchy = new(roots);

        foreach (string root in Roots)
        {
            NodePath rootPath = new(root);
            var rootBuilder = GetNodeFromPath(rootPath)
                ?? GetMissingNode(rootPath, missingChildBehavior);
            var rootNode = BuildNode(null, rootBuilder, missingChildBehavior);
            roots.Add(rootNode);
        }

        return hierarchy;
    }

    private AstNode BuildNode(AstNode? parent, AstNodeBuilder builder, MissingChildBehavior missingChildBehavior)
    {
        var path = builder.Path;

        Dictionary<string, AstNode> children = new();
        var members = builder.Members
            .ToDictionary(b => b.Name, b => b.Build());
        AstNode node = new(
            builder.Name,
            builder.Documentation,
            parent,
            children,
            members,
            new HashSet<object>(builder.Attributes)
        );

        foreach (string child in builder.Children)
        {
            var childPath = path.CreateLeafPath(child);
            var childBuilder = GetNodeFromPath(childPath)
                ?? GetMissingNode(childPath, missingChildBehavior);
            var childNode = BuildNode(node, childBuilder, missingChildBehavior);
            children.Add(childNode.Name, childNode);
        }

        return node;
    }

    private AstNodeBuilder GetMissingNode(NodePath path, MissingChildBehavior missingChildBehavior)
    {        
        if (missingChildBehavior == MissingChildBehavior.Throw)
        {
            throw new InvalidOperationException($"Node '{path}' does not exist");
        }

        return CreateNode(path);
    }

    internal AstNode BuildNode(AstNodeBuilder builder, MissingChildBehavior missingChildBehavior)
    {
        var path = builder.Path;
        string root = path.IsRoot ? path.Leaf : path.Root;
        NodePath rootPath = new(root);

        if (!builders.ContainsKey(path)) throw nodeDoesNotExist(path);
        if (!builders.TryGetValue(rootPath, out var rootBuilder)) throw nodeDoesNotExist(rootPath);

        var rootNode = BuildNode(null, rootBuilder, missingChildBehavior);

        return rootNode.GetDecendantNodeFromPath(path, true)
            ?? throw nodeDoesNotExist(path);

        static InvalidOperationException nodeDoesNotExist(NodePath path) =>
            new($"The node '{path}' does not exist in the hierarchy");
    }

    /// <summary>
    /// Implicitly converts an <see cref="AstNodeBuilder"/> to an <see cref="AstNodeHierarchy"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator AstNodeHierarchy(AstNodeHierarchyBuilder builder) =>
        builder.Build();
}
