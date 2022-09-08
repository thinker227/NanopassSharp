using System;

// I won't type "AstNodeHierarchyBuilder" a million times throughout this file
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
    public void Build_ReturnsCorrectTree()
    {
        TreeBuilder builder = new();
        builder.AddRoot("ast")
            .WithChildren(new[] { "stmt", "expr" });
        builder.CreateNode("ast.stmt")
            .WithChildren(new[] { "var", "if", "exprStmt" });
        builder.CreateNode("ast.stmt.var");
        builder.CreateNode("ast.stmt.if");
        builder.CreateNode("ast.stmt.exprStmt");
        builder.CreateNode("ast.expr")
            .WithChildren(new[] { "unary", "binary", "literal" });
        builder.CreateNode("ast.expr.unary");
        builder.CreateNode("ast.expr.binary");
        builder.CreateNode("ast.expr.literal");

        var tree = builder.Build(MissingChildBehavior.Throw);



        tree.Roots.Count.ShouldBe(1);

        {
            var ast = tree.Roots[0];
            ast.Name.ShouldBe("ast");
            ast.Parent.ShouldBeNull();
            ast.Children.Count.ShouldBe(2);

            {
                var stmt = ast.Children["stmt"];
                stmt.Name.ShouldBe("stmt");
                stmt.Parent.ShouldBeSameAs(ast);
                stmt.Children.Count.ShouldBe(3);

                {
                    var var = stmt.Children["var"];
                    var.Children.ShouldBeEmpty();
                    var.Parent.ShouldBeSameAs(stmt);
                    var.Name.ShouldBe("var");
                }

                {
                    var @if = stmt.Children["if"];
                    @if.Children.ShouldBeEmpty();
                    @if.Parent.ShouldBeSameAs(stmt);
                    @if.Name.ShouldBe("if");
                }

                {
                    var exprStmt = stmt.Children["exprStmt"];
                    exprStmt.Children.ShouldBeEmpty();
                    exprStmt.Parent.ShouldBeSameAs(stmt);
                    exprStmt.Name.ShouldBe("exprStmt");
                }
            }

            {
                var expr = ast.Children["expr"];
                expr.Name.ShouldBe("expr");
                expr.Parent.ShouldBeSameAs(ast);
                expr.Children.Count.ShouldBe(3);

                {
                    var unary = expr.Children["unary"];
                    unary.Children.ShouldBeEmpty();
                    unary.Parent.ShouldBeSameAs(expr);
                    unary.Name.ShouldBe("unary");
                }

                {
                    var binary = expr.Children["binary"];
                    binary.Children.ShouldBeEmpty();
                    binary.Parent.ShouldBeSameAs(expr);
                    binary.Name.ShouldBe("binary");
                }

                {
                    var literal = expr.Children["literal"];
                    literal.Children.ShouldBeEmpty();
                    literal.Parent.ShouldBeSameAs(expr);
                    literal.Name.ShouldBe("literal");
                }
            }
        }
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
}
