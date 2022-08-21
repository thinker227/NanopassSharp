using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for an <see cref="AstNode"/>.
/// </summary>
public sealed class AstNodeBuilder
{
    private AstNodeBuilder? parent;
    private readonly Dictionary<string, AstNodeBuilder> childrenBuilders;
    private readonly Dictionary<string, AstNodeMemberBuilder> memberBuilders;

    /// <summary>
    /// <inheritdoc cref="AstNode.Name" path="/summary"/>
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// <inheritdoc cref="AstNode.Documentation" path="/summary"/>
    /// </summary>
    public string? Documentation { get; set; }
    /// <summary>
    /// <inheritdoc cref="AstNode.Parent" path="/summary"/>
    /// </summary>
    /// <remarks>This property builds the entire node in order to retrieve the parent.</remarks>
    public AstNode? Parent =>
        Build().Parent;
    /// <summary>
    /// <inheritdoc cref="AstNode.Children" path="/summary"/>
    /// </summary>
    /// <remarks>This property builds the entire node in order to retrieve the children.</remarks>
    public IReadOnlyDictionary<string, AstNode> Children =>
        Build().Children;
    /// <summary>
    /// <inheritdoc cref="AstNode.Members" path="/summary"/>
    /// </summary>
    /// <remarks>This property builds all members.</remarks>
    public IReadOnlyDictionary<string, AstNodeMember> Members =>
        memberBuilders.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Build());
    /// <summary>
    /// <inheritdoc cref="AstNode.Attributes" path="/summary"/>
    /// </summary>
    public ISet<object> Attributes { get; set; }



    /// <summary>
    /// Initializes a new <see cref="AstNodeBuilder"/> instance.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    public AstNodeBuilder(string name)
    {
        Name = name;
        Documentation = null;
        parent = null;
        childrenBuilders = new();
        memberBuilders = new();
        Attributes = new HashSet<object>();
    }



    /// <summary>
    /// Sets the name of the node.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder WithName(string name)
    {
        Name = name;
        return this;
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
    /// Adds a child to the node.
    /// </summary>
    /// <param name="name">The name of the child.</param>
    /// <returns>A new builder for the child node.</returns>
    public AstNodeBuilder AddChild(string name, string? documentation = null)
    {
        AstNodeBuilder nodeBuilder = new(name)
        {
            parent = this,
            Documentation = documentation
        };
        childrenBuilders.Add(name, nodeBuilder);
        return nodeBuilder;
    }
    /// <summary>
    /// Adds a member to the node.
    /// </summary>
    /// <param name="member">The member to add.</param>
    /// <returns>The current builder.</returns>
    public AstNodeBuilder AddMember(AstNodeMember member)
    {
        memberBuilders.Add(member.Name, new AstNodeMemberBuilder(member));
        return this;
    }
    /// <summary>
    /// Adds a member to the node.
    /// </summary>
    /// <param name="name">The name of the member.</param>
    /// <param name="documentation">The documentation of the member.</param>
    /// <param name="type">The type of the member.</param>
    /// <returns>A new builder for the member.</returns>
    public AstNodeMemberBuilder AddMember(string name, string? documentation = null, string? type = null)
    {
        var memberBuilder = new AstNodeMemberBuilder(name).WithDocumentation(documentation).WithType(type);
        memberBuilders.Add(name, memberBuilder);
        return memberBuilder;
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
    /// Builds the current node "upwards", primarily building the parent node, followed by child nodes.
    /// Any child builders with the same name as <paramref name="child"/> are ignored.
    /// In successive recursive calls, <paramref name="child"/> is always null.
    /// </summary>
    private (AstNode node, Dictionary<string, AstNode> children) BuildUp(AstNodeBuilder? child)
    {
        var parentNode = parent?.BuildUp(this).node;
        Dictionary<string, AstNode> children = new();
        AstNode node = new(Name, Documentation, parentNode, children, Members, new HashSet<object>(Attributes));
        foreach (var (name, builder) in childrenBuilders)
        {
            if (name == child?.Name) continue;
            children.Add(name, builder.BuildDown(node));
        }
        return (node, children);
    }
    /// <summary>
    /// Builds the current node "downwards", only building children as a parent node is already provided.
    /// </summary>
    private AstNode BuildDown(AstNode? parent)
    {
        Dictionary<string, AstNode> children = new();
        AstNode node = new(Name, Documentation, parent, children, Members, new HashSet<object>(Attributes));
        foreach (var (name, builder) in childrenBuilders)
        {
            children.Add(name, builder.BuildDown(node));
        }
        return node;
    }
    /// <summary>
    /// Builds an <see cref="AstNode"/> from the builder.
    /// </summary>
    public AstNode Build()
    {
        var parentNode = parent?.BuildUp(this);
        var node = BuildDown(parentNode?.node);
        parentNode?.children.Add(node.Name, node);
        return node;
    }
    /// <summary>
    /// Implicitly converts an <see cref="AstNodeBuilder"/>
    /// into an <see cref="AstNode"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator AstNode(AstNodeBuilder builder) =>
        builder.Build();
}
