using TreeBuilder = NanopassSharp.Builders.AstNodeHierarchyBuilder;
using NodeBuilder = NanopassSharp.Builders.AstNodeBuilder;

using System.Collections.Generic;

namespace NanopassSharp.Builders.Tests;

public class AstNodeBuilderTests
{
    [Fact]
    public void WithDocumentation_SetsDocumentation()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        node.WithDocumentation("docs");

        node.Documentation.ShouldBe("docs");
    }
    [Fact]
    public void WithDocumentation_ReturnsSelf()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        var withDocumentation = node.WithDocumentation("docs");

        withDocumentation.ShouldBeSameAs(node);
    }

    [Fact]
    public void AddAttribute_AddsAttribute()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        node.AddAttribute("attribute");
        node.AddAttribute(27);
        node.AddAttribute(true);

        object[] expected = new object[] { "attribute", 27, true };
        node.Attributes.ShouldBe(expected);
    }
    [Fact]
    public void AddAttribute_ReturnsSelf()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        var addAttribute = node.AddAttribute("attribute");

        addAttribute.ShouldBeSameAs(node);
    }

    [Fact]
    public void WithAttributes_SetsAttributes()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        object[] attributes = new object[] { "attribute", 27, true };
        node.WithAttributes(new HashSet<object>(attributes));

        node.Attributes.ShouldBe(attributes);
    }
    [Fact]
    public void WithAttributes_ReturnsSelf()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        object[] attributes = new object[] { "attribute", 27, true };
        var withAttributes = node.WithAttributes(new HashSet<object>(attributes));

        withAttributes.ShouldBeSameAs(node);
    }

    [Fact]
    public void AddChild_AddsChild()
    {
        var node = new TreeBuilder()
            .CreateNode("a");
        node.AddChild("b");

        IEnumerable<string> expected = new[] { "b" };
        node.Children.ShouldBe(expected);
    }
    [Fact]
    public void AddsChild_ReturnsChildBuilder()
    {
        var node = new TreeBuilder()
            .CreateNode("a");
        var child = node.AddChild("b");

        var expectedPath = NodePath.ParseUnsafe("a.b");
        child.Path.ShouldBe(expectedPath);
        child.Parent.ShouldBe("a");
    }
}
