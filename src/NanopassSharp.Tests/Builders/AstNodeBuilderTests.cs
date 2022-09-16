using System.Collections.Generic;
using TreeBuilder = NanopassSharp.Builders.AstNodeHierarchyBuilder;

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

    [Fact]
    public void AddMember_AddsMember()
    {
        TreeBuilder builder = new();
        var fooBuilder = builder.AddRoot("foo");
        fooBuilder.AddMember("a");
        fooBuilder.AddMember("b");
        fooBuilder.AddMember("c");

        var tree = builder.Build();



        tree.Roots.Count.ShouldBe(1);

        {
            var foo = tree.Roots[0];
            foo.Members.Count.ShouldBe(3);
        }
    }

    [Fact]
    public void RemoveMember_RemovesMember_WithStringParameter()
    {
        TreeBuilder builder = new();
        var fooBuilder = builder.AddRoot("foo");
        fooBuilder.AddMember("a");
        fooBuilder.AddMember("b");
        fooBuilder.AddMember("c");
        fooBuilder.RemoveMember("a");
        fooBuilder.RemoveMember("b");

        var tree = builder.Build();



        tree.Roots.Count.ShouldBe(1);

        {
            var foo = tree.Roots[0];
            foo.Members.Count.ShouldBe(1);
        }
    }
    [Fact]
    public void RemoveMember_RemovesNodeMember_WithNodeParameter()
    {
        TreeBuilder builder = new();
        var fooBuilder = builder.AddRoot("foo");
        var a = fooBuilder.AddMember("a");
        var b = fooBuilder.AddMember("b");
        fooBuilder.AddMember("c");
        fooBuilder.RemoveMember(a);
        fooBuilder.RemoveMember(b);

        var tree = builder.Build();



        tree.Roots.Count.ShouldBe(1);

        {
            var foo = tree.Roots[0];
            foo.Members.Count.ShouldBe(1);
        }
    }
}
