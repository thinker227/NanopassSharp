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

        Assert.Equal("docs", node.Documentation);
    }
    [Fact]
    public void WithDocumentation_ReturnsSelf()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        var withDocumentation = node.WithDocumentation("docs");

        Assert.True(ReferenceEquals(node, withDocumentation));
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
        Assert.Equal(expected, node.Attributes);
    }
    [Fact]
    public void AddAttribute_ReturnsSelf()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        var addAttribute = node.AddAttribute("attribute");

        Assert.True(ReferenceEquals(node, addAttribute));
    }

    [Fact]
    public void WithAttributes_SetsAttributes()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        object[] attributes = new object[] { "attribute", 27, true };
        node.WithAttributes(new HashSet<object>(attributes));

        Assert.Equal(attributes, node.Attributes);
    }
    [Fact]
    public void WithAttributes_ReturnsSelf()
    {
        var node = new TreeBuilder()
            .CreateNode("foo");
        object[] attributes = new object[] { "attribute", 27, true };
        var withAttributes = node.WithAttributes(new HashSet<object>(attributes));

        Assert.True(ReferenceEquals(node, withAttributes));
    }

    [Fact]
    public void AddChild_AddsChild()
    {
        var node = new TreeBuilder()
            .CreateNode("a");
        node.AddChild("b");

        string[] expected = new[] { "b" };

        Assert.Equal(expected, (IEnumerable<string>)node.Children);
    }
    [Fact]
    public void AddsChild_ReturnsChildBuilder()
    {
        var node = new TreeBuilder()
            .CreateNode("a");
        var child = node.AddChild("b");

        var expectedPath = NodePath.ParseUnsafe("a.b");
        Assert.Equal(expectedPath, child.Path);
        Assert.Equal("a", child.Parent);
    }
}
