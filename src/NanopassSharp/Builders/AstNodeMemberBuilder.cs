using System.Collections.Generic;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for an <see cref="AstNodeMember"/>.
/// </summary>
public sealed class AstNodeMemberBuilder
{
    /// <summary>
    /// <inheritdoc cref="AstNodeMember.Name" path="/summary"/>
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// <inheritdoc cref="AstNodeMember.Documentation" path="/summary"/>
    /// </summary>
    public string? Documentation { get; set; }
    /// <summary>
    /// <inheritdoc cref="AstNodeMember.Type" path="/summary"/>
    /// </summary>
    public string? Type { get; set; }
    /// <summary>
    /// <inheritdoc cref="AstNodeMember.Attributes" path="/summary"/>
    /// </summary>
    public ISet<object> Attributes { get; set; }



    /// <summary>
    /// Initializes a new <see cref="AstNodeBuilder"/> instance.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    public AstNodeMemberBuilder(string name)
    {
        Name = name;
        Documentation = null;
        Type = null;
        Attributes = new HashSet<object>();
    }
    /// <summary>
    /// Initializes a new <see cref="AstNodeBuilder"/> instance.
    /// </summary>
    /// <param name="member">The <see cref="AstNodeMember"/> to create the builder from.</param>
    public AstNodeMemberBuilder(AstNodeMember member)
    {
        Name = member.Name;
        Documentation = member.Documentation;
        Type = member.Type;
        Attributes = new HashSet<object>(member.Attributes);
    }



    /// <summary>
    /// Sets the name of the member.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public AstNodeMemberBuilder WithName(string name)
    {
        Name = name;
        return this;
    }
    /// <summary>
    /// Sets the documentation of the member.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Documentation" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public AstNodeMemberBuilder WithDocumentation(string? documentation)
    {
        Documentation = documentation;
        return this;
    }
    /// <summary>
    /// Sets the type of the member.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Type" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public AstNodeMemberBuilder WithType(string? type)
    {
        Type = type;
        return this;
    }
    /// <summary>
    /// Adds an attribute to the member.
    /// </summary>
    /// <param name="name">The attribute to add.</param>
    /// <returns>The current builder.</returns>
    public AstNodeMemberBuilder AddAttribute(object attribute)
    {
        Attributes.Add(attribute);
        return this;
    }

    /// <summary>
    /// Builds an <see cref="AstNodeMember"/> from the builder.
    /// </summary>
    public AstNodeMember Build() =>
        new(Name, Documentation, Type, new HashSet<object>(Attributes));
    /// <summary>
    /// Implicitly converts an <see cref="AstNodeMemberBuilder"/> to an <see cref="AstNodeMember"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator AstNodeMember(AstNodeMemberBuilder builder) =>
        builder.Build();
}
