using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for an <see cref="AstNode"/>.
/// </summary>
public sealed class AstNodeBuilder
{
    private readonly List<AstNodeMemberBuilder> members;

    /// <summary>
    /// The hierarchy this node is a part of.
    /// </summary>
    public AstNodeHierarchyBuilder Hierarchy { get; }

    /// <summary>
    /// The path to this node.
    /// </summary>
    public NodePath Path { get; }

    /// <summary>
    /// <inheritdoc cref="AstNode.Name" path="/summary"/>
    /// </summary>
    public string Name => Path.Leaf;

    /// <summary>
    /// <inheritdoc cref="AstNode.Documentation" path="/summary"/>
    /// </summary>
    public string? Documentation { get; set; }

    /// <summary>
    /// <inheritdoc cref="AstNode.Parent" path="/summary"/>
    /// </summary>
    public string? Parent => Path.Parent.Leaf;

    /// <summary>
    /// <inheritdoc cref="AstNode.Children" path="/summary"/>
    /// </summary>
    public ICollection<string> Children { get; set; }

    /// <summary>
    /// <inheritdoc cref="AstNode.Members" path="/summary"/>
    /// </summary>
    public IReadOnlyCollection<AstNodeMemberBuilder> Members => members;

    /// <summary>
    /// <inheritdoc cref="AstNode.Attributes" path="/summary"/>
    /// </summary>
    public ISet<object> Attributes { get; set; }



    internal AstNodeBuilder(AstNodeHierarchyBuilder hierarchy, NodePath path)
    {
        members = new();
        Hierarchy = hierarchy;
        Path = path;
        Documentation = null;
        Children = new List<string>();
        Attributes = new HashSet<object>();
    }



    /// <summary>
    /// Sets the documentation of the node.
    /// </summary>
    /// <param name="documentation"><inheritdoc cref="Documentation" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder WithDocumentation(string? documentation)
    {
        Documentation = documentation;
        return this;
    }

    /// <summary>
    /// Adds an attribute to the node.
    /// </summary>
    /// <param name="attribute">The attribute to add.</param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder AddAttribute(object attribute)
    {
        Attributes.Add(attribute);
        return this;
    }

    /// <summary>
    /// Removes an attribute from the node.
    /// </summary>
    /// <param name="attribute">The attribute to remove.</param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder RemoveAttribute(object attribute)
    {
        Attributes.Remove(attribute);
        return this;
    }

    /// <summary>
    /// Sets the attributes of the node.
    /// </summary>
    /// <param name="attributes"><inheritdoc cref="Attributes" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder WithAttributes(ISet<object> attributes)
    {
        Attributes = attributes;
        return this;
    }

    /// <summary>
    /// Adds a child to the node.
    /// </summary>
    /// <param name="name">The name of the child node.</param>
    /// <returns>A builder for the child node.</returns>
    public AstNodeBuilder AddChild(string name)
    {
        var builder = Hierarchy.CreateNode(name, Path);
        Children.Add(builder.Name);
        return builder;
    }

    /// <summary>
    /// Adds an already existing node as a child to the node.
    /// </summary>
    /// <param name="node">The child node to add.</param>
    /// <returns>A builder for the child node.</returns>
    public AstNodeBuilder AddChild(AstNode node)
    {
        var builder = Hierarchy.CreateNodeWithParent(node, Path);
        Children.Add(builder.Name);
        return builder;
    }

    /// <summary>
    /// Removes a child from the node.
    /// </summary>
    /// <param name="name">The name of the child to remove.</param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder RemoveChild(string name)
    {
        Hierarchy.RemoveNode(Path);
        Children.Remove(name);

        return this;
    }

    /// <summary>
    /// Sets the children of the node.
    /// </summary>
    /// <param name="children"><inheritdoc cref="Children" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder WithChildren(IEnumerable<string> children)
    {
        Children = children is ICollection<string> collection
            ? collection
            : children.ToList();
        return this;
    }

    /// <summary>
    /// Adds a member to the node.
    /// </summary>
    /// <param name="name">The name of the member.</param>
    /// <returns>A builder for the member.</returns>
    public AstNodeMemberBuilder AddMember(string name)
    {
        AstNodeMemberBuilder builder = new(name);
        members.Add(builder);
        return builder;
    }

    /// <summary>
    /// Adds a member to the node.
    /// </summary>
    /// <param name="member">The member to add.</param>
    /// <returns>A builder for the member.</returns>
    public AstNodeMemberBuilder AddMember(AstNodeMember member) => AddMember(member.Name)
        .WithDocumentation(member.Documentation)
        .WithType(member.Type)
        .WithAttributes(new HashSet<object>(member.Attributes));

    /// <summary>
    /// Removes a member from the node.
    /// </summary>
    /// <param name="name">The name of the member to remove.</param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder RemoveMember(string name)
    {
        members.RemoveAll(m => m.Name == name);
        return this;
    }

    /// <summary>
    /// Removes a member from the node.
    /// </summary>
    /// <param name="builder">The builder to remove.</param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder RemoveMember(AstNodeMemberBuilder builder)
    {
        members.Remove(builder);
        return this;
    }

    /// <summary>
    /// Builds the current node.
    /// </summary>
    /// <param name="missingChildBehavior">The behavior if a node is missing.</param>
    /// <remarks>
    /// In order to build the current node, the entire hierarchy
    /// is built from the root of the current builder.
    /// This means that the root of the current node is built along with all its children.
    /// </remarks>
    public AstNode Build(MissingChildBehavior missingChildBehavior = MissingChildBehavior.Throw) =>
        Hierarchy.BuildNode(this, missingChildBehavior);
}
