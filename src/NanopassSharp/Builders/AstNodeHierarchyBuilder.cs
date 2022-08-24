using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for an <see cref="AstNodeHierarchy"/>.
/// </summary>
public sealed class AstNodeHierarchyBuilder
{
    private readonly List<AstNodeBuilder> rootBuilders;

    /// <summary>
    /// <inheritdoc cref="AstNodeHierarchy.Roots"/>
    /// </summary>
    public IEnumerable<AstNode> Roots =>
        rootBuilders.Select(b => b.Build());



    /// <summary>
    /// Initializes a new <see cref="AstNodeHierarchyBuilder"/> instance.
    /// </summary>
    public AstNodeHierarchyBuilder()
    {
        rootBuilders = new();
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
            builder.AddRoot(root);
        }
        return builder;
    }

    /// <summary>
    /// Adds a root node to the hierarchy.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="documentation">The documentation of the node.</param>
    /// <returns>A new builder for the node.</returns>
    public AstNodeBuilder AddRoot(string name, string? documentation = null)
    {
        var builder = new AstNodeBuilder(name).WithDocumentation(documentation);
        return AddRoot(builder);
    }
    /// <summary>
    /// Adds a root node to the hierarchy.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="builderAction">An action to apply to the new builder.</param>
    /// <returns>The current builder.</returns>
    public AstNodeHierarchyBuilder AddRoot(string name, Action<AstNodeBuilder> builderAction)
    {
        AstNodeBuilder builder = new(name);
        AddRoot(builder);
        builderAction(builder);
        return this;
    }
    /// <summary>
    /// Adds a root node to the hierarchy.
    /// </summary>
    /// <param name="root">The root node to add.</param>
    /// <returns>The current builder.</returns>
    public AstNodeHierarchyBuilder AddRoot(AstNode root)
    {
        var builder = AstNodeBuilder.FromNode(root);
        AddRoot(builder);
        return this;
    }
    private AstNodeBuilder AddRoot(AstNodeBuilder root)
    {
        rootBuilders.Add(root);
        return root;
    }

    /// <summary>
    /// Builds an <see cref="AstNodeHierarchy"/> from the builder.
    /// </summary>
    public AstNodeHierarchy Build() =>
        rootBuilders.Count == 0 ? AstNodeHierarchy.Empty : (new(Roots.ToList()));
    /// <summary>
    /// Implicitly converts an <see cref="AstNodeBuilder"/> to an <see cref="AstNodeHierarchy"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator AstNodeHierarchy(AstNodeHierarchyBuilder builder) =>
        builder.Build();
}
