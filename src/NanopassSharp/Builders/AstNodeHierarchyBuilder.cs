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
    /// Adds a root node to the hierarchy.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="documentation">The documentation of the node.</param>
    /// <returns>A new builder for the node.</returns>
    public AstNodeBuilder AddRoot(string name, string? documentation = null)
    {
        var nodeBuilder = new AstNodeBuilder(name).WithDocumentation(documentation);
        rootBuilders.Add(nodeBuilder);
        return nodeBuilder;
    }

    /// <summary>
    /// Builds an <see cref="AstNodeHierarchy"/> from the builder.
    /// </summary>
    public AstNodeHierarchy Build() => new(Roots.ToList());
    /// <summary>
    /// Implicitly converts an <see cref="AstNodeBuilder"/> to an <see cref="AstNodeHierarchy"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator AstNodeHierarchy(AstNodeHierarchyBuilder builder) =>
        builder.Build();
}
