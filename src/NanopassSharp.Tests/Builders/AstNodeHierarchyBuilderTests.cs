using System;
using System.Collections.Generic;
using TreeBuilder = NanopassSharp.Builders.AstNodeHierarchyBuilder;

namespace NanopassSharp.Builders.Tests;

public class AstNodeHierarchyBuilderTests
{
    [Fact]
    public void Ctor_SetsRoots()
    {
        TreeBuilder builder = new();

        builder.Roots.ShouldNotBeNull();
    }

    [InlineData("a")]
    [InlineData("a.b.c")]
    [Theory]
    public void CreateNode_ReturnsCorrectNode(string pathString)
    {
        TreeBuilder builder = new();
        var path = NodePath.ParseUnsafe(pathString);
        var node = builder.CreateNode(path);

        node.Hierarchy.ShouldBe(builder);
        node.Path.ShouldBe(path);
        node.Documentation.ShouldBeNull();
        node.Children.ShouldBeEmpty();
        node.Members.ShouldBeEmpty();
        node.Attributes.ShouldBeEmpty();
    }

    [Fact]
    public void AddRoot_ReturnsCorrectNode()
    {
        TreeBuilder builder = new();
        var root = builder.AddRoot("a");

        NodePath expectedPath = new("a");

        root.Hierarchy.ShouldBe(builder);
        root.Path.ShouldBe(expectedPath);
        root.Documentation.ShouldBeNull();
        root.Children.ShouldBeEmpty();
        root.Members.ShouldBeEmpty();
        root.Attributes.ShouldBeEmpty();
    }

    [Fact]
    public void AddRoot_AddsRoot()
    {
        TreeBuilder builder = new();
        builder.AddRoot("a");

        string[] expected = new[] { "a" };
        builder.Roots.ShouldBe(expected);
    }

    [Fact]
    public void GetNodeFromPath_ReturnsCorrectNode()
    {
        TreeBuilder builder = new();
        builder.CreateNode("a");
        var node = builder.CreateNode("a.b");
        builder.CreateNode("a.b.c");
        builder.CreateNode("d.e");

        var actual = builder.GetNodeFromPath(NodePath.ParseUnsafe("a.b"));
        actual?.Path.ShouldBe(node.Path);
    }

    [Fact]
    public void GetNodeFromPath_ReturnsNull_WhenNodeDoesNotExist()
    {
        TreeBuilder builder = new();
        builder.CreateNode("a");
        builder.CreateNode("a.b");
        builder.CreateNode("d.e");

        var actual = builder.GetNodeFromPath(NodePath.ParseUnsafe("a.b.c"));
        actual.ShouldBeNull();
    }
    
    [Fact]
    public void Build_BuildsChildren()
    {
        TreeBuilder builder = new();
        builder.AddRoot("a")
            .WithChildren(new[] { "b", "c" });
        builder.CreateNode("a.b")
            .WithChildren(new[] { "d", "e", "f" });
        builder.CreateNode("a.c");
        builder.CreateNode("a.b.d");
        builder.CreateNode("a.b.e");
        builder.CreateNode("a.b.f");

        var tree = builder.Build();



        tree.Roots.Count.ShouldBe(1);

        {
            var a = tree.Roots[0];
            a.Name.ShouldBe("a");
            a.Parent.ShouldBeNull();
            a.Children.Count.ShouldBe(2);
            a.Children.Keys.ShouldContain("b");
            a.Children.Keys.ShouldContain("c");

            {
                var b = a.Children["b"];
                b.Name.ShouldBe("b");
                b.Parent.ShouldBeSameAs(a);
                b.Children.Count.ShouldBe(3);
                b.Children.Keys.ShouldContain("d");
                b.Children.Keys.ShouldContain("e");
                b.Children.Keys.ShouldContain("f");

                {
                    var d = b.Children["d"];
                    d.Name.ShouldBe("d");
                    d.Parent.ShouldBeSameAs(b);
                    d.Children.ShouldBeEmpty();
                }

                {
                    var e = b.Children["e"];
                    e.Name.ShouldBe("e");
                    e.Parent.ShouldBeSameAs(b);
                    e.Children.ShouldBeEmpty();
                }

                {
                    var f = b.Children["f"];
                    f.Name.ShouldBe("f");
                    f.Parent.ShouldBeSameAs(b);
                    f.Children.ShouldBeEmpty();
                }
            }

            {
                var c = a.Children["c"];
                c.Name.ShouldContain("c");
                c.Parent.ShouldBeSameAs(a);
                c.Children.ShouldBeEmpty();
            }
        }
    }

    [Fact]
    public void Build_Throws_WhenMissingChildWithThrowBehavior()
    {
        TreeBuilder builder = new();
        builder.AddRoot("a")
            .WithChildren(new[] { "b" });

        Should.Throw<InvalidOperationException>(() => builder.Build(MissingChildBehavior.Throw));
    }

    [Fact]
    public void Build_CreatesMissingChild_WhenMissingChildWithCreateBehavior()
    {
        TreeBuilder builder = new();
        builder.AddRoot("a")
            .WithChildren(new[] { "b" });

        var tree = builder.Build(MissingChildBehavior.CreateEmptyNode);



        tree.Roots.Count.ShouldBe(1);

        {
            var a = tree.Roots[0];
            a.Name.ShouldBe("a");
            a.Parent.ShouldBeNull();
            a.Children.Count.ShouldBe(1);
            a.Children.Keys.ShouldContain("b");

            {
                var b = a.Children["b"];
                b.Name.ShouldBe("b");
                b.Parent.ShouldBeSameAs(a);
                b.Children.ShouldBeEmpty();
            }
        }
    }

    [Fact]
    public void Build_BuildsAllRoots()
    {
        TreeBuilder builder = new();
        builder.AddRoot("a");
        builder.AddRoot("b");
        builder.AddRoot("c");

        var tree = builder.Build();



        builder.Roots.Count.ShouldBe(3);

        {
            var a = tree.Roots[0];
            a.Name.ShouldBe("a");
            a.Parent.ShouldBeNull();
            a.Children.ShouldBeEmpty();
        }
        {
            var b = tree.Roots[1];
            b.Name.ShouldBe("b");
            b.Parent.ShouldBeNull();
            b.Children.ShouldBeEmpty();
        }
        {
            var c = tree.Roots[2];
            c.Name.ShouldBe("c");
            c.Parent.ShouldBeNull();
            c.Children.ShouldBeEmpty();
        }
    }

    [Fact]
    public void Build_BuildsMembers()
    {
        TreeBuilder builder = new();
        var fooBuilder = builder.AddRoot("foo");
        fooBuilder.AddMember("a")
            .WithType("type a")
            .WithDocumentation("docs a")
            .WithAttributes(new HashSet<object>(new object[] { "attribute a", 0, true }));
        fooBuilder.AddMember("b")
            .WithType("type b")
            .WithDocumentation("docs b")
            .WithAttributes(new HashSet<object>(new object[] { "attribute b", 1, false }));
        fooBuilder.AddMember("c")
            .WithType("type c")
            .WithDocumentation("docs c")
            .WithAttributes(new HashSet<object>(new object[] { "attribute c", 2, true }));

        var tree = builder.Build();



        tree.Roots.Count.ShouldBe(1);

        {
            var foo = tree.Roots[0];
            foo.Members.Count.ShouldBe(3);
            foo.Members.Keys.ShouldContain("a");
            foo.Members.Keys.ShouldContain("b");
            foo.Members.Keys.ShouldContain("c");

            {
                var a = foo.Members["a"];
                a.Name.ShouldBe("a");
                a.Type.ShouldBe("type a");
                a.Documentation.ShouldBe("docs a");
                a.Attributes.ShouldBe(new object[] { "attribute a", 0, true });
            }

            {
                var b = foo.Members["b"];
                b.Name.ShouldBe("b");
                b.Type.ShouldBe("type b");
                b.Documentation.ShouldBe("docs b");
                b.Attributes.ShouldBe(new object[] { "attribute b", 1, false });
            }

            {
                var c = foo.Members["c"];
                c.Name.ShouldBe("c");
                c.Type.ShouldBe("type c");
                c.Documentation.ShouldBe("docs c");
                c.Attributes.ShouldBe(new object[] { "attribute c", 2, true });
            }
        }
    }

    private static IEnumerable<object[]> BuildNode_BuildsNode_Data()
    {
        {
            TreeBuilder builder = new();
            var foo = builder.AddRoot("foo");

            yield return new object[]
            {
                builder,
                foo,
                "foo"
            };
        }

        {
            TreeBuilder builder = new();
            var a = builder.AddRoot("a");
            var b = a.AddChild("b");
            var c = b.AddChild("c");
            var d = c.AddChild("d");
            var e = d.AddChild("e");
            var f = e.AddChild("f");

            yield return new object[]
            {
                builder,
                f,
                "f"
            };
        }
    }

    [MemberData(nameof(BuildNode_BuildsNode_Data))]
    [Theory]
    public void BuildNode_BuildsNode(TreeBuilder treeBuilder, AstNodeBuilder nodeBuilder, string expectedName)
    {
        var node = treeBuilder.BuildNode(nodeBuilder, MissingChildBehavior.Throw);

        node.Name.ShouldBe(expectedName);
    }
}
