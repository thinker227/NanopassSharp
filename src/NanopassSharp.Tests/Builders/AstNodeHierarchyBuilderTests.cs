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

        Assert.NotNull(builder.Roots);
    }

    [InlineData("a")]
    [InlineData("a.b.c")]
    [Theory]
    public void CreateNode_ReturnsCorrectNode(string pathString)
    {
        TreeBuilder builder = new();
        var path = NodePath.ParseUnsafe(pathString);
        var node = builder.CreateNode(path);

        Assert.Equal(builder, node.Hierarchy);
        Assert.Equal(path, node.Path);
        Assert.Null(node.Documentation);
        Assert.Empty(node.Children);
        Assert.Empty(node.Members);
        Assert.Empty(node.Attributes);
    }

    [Fact]
    public void AddRoot_ReturnsCorrectNode()
    {
        TreeBuilder builder = new();
        var root = builder.AddRoot("a");

        NodePath expectedPath = new("a");

        Assert.Equal(builder, root.Hierarchy);
        Assert.Equal(expectedPath, root.Path);
        Assert.Null(root.Documentation);
        Assert.Empty(root.Children);
        Assert.Empty(root.Members);
        Assert.Empty(root.Attributes);
    }
    [Fact]
    public void AddRoot_AddsRoot()
    {
        TreeBuilder builder = new();
        builder.AddRoot("a");

        string[] expected = new[] { "a" };
        Assert.Equal(expected, builder.Roots);
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
        Assert.Equal(node.Path, actual?.Path);
    }
    [Fact]
    public void GetNodeFromPath_ReturnsNull_WhenNodeDoesNotExist()
    {
        TreeBuilder builder = new();
        builder.CreateNode("a");
        builder.CreateNode("a.b");
        builder.CreateNode("d.e");

        var actual = builder.GetNodeFromPath(NodePath.ParseUnsafe("a.b.c"));
        Assert.Null(actual);
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



        Assert.Equal(1, tree.Roots.Count);

        {
            var ast = tree.Roots[0];
            Assert.Equal("ast", ast.Name);
            Assert.Null(ast.Parent);
            Assert.Equal(2, ast.Children.Count);

            {
                var stmt = ast.Children["stmt"];
                Assert.Equal("stmt", stmt.Name);
                Assert.True(ReferenceEquals(ast, stmt.Parent));
                Assert.Equal(3, stmt.Children.Count);

                {
                    var var = stmt.Children["var"];
                    Assert.Empty(var.Children);
                    Assert.True(ReferenceEquals(stmt, var.Parent));
                    Assert.Equal("var", var.Name);
                }

                {
                    var @if = stmt.Children["if"];
                    Assert.Empty(@if.Children);
                    Assert.True(ReferenceEquals(stmt, @if.Parent));
                    Assert.Equal("if", @if.Name);
                }

                {
                    var exprStmt = stmt.Children["exprStmt"];
                    Assert.Empty(exprStmt.Children);
                    Assert.True(ReferenceEquals(stmt, exprStmt.Parent));
                    Assert.Equal("exprStmt", exprStmt.Name);
                }
            }

            {
                var expr = ast.Children["expr"];
                Assert.Equal("expr", expr.Name);
                Assert.True(ReferenceEquals(ast, expr.Parent));
                Assert.Equal(3, expr.Children.Count);

                {
                    var unary = expr.Children["unary"];
                    Assert.Empty(unary.Children);
                    Assert.True(ReferenceEquals(expr, unary.Parent));
                    Assert.Equal("unary", unary.Name);
                }

                {
                    var binary = expr.Children["binary"];
                    Assert.Empty(binary.Children);
                    Assert.True(ReferenceEquals(expr, binary.Parent));
                    Assert.Equal("binary", binary.Name);
                }

                {
                    var literal = expr.Children["literal"];
                    Assert.Empty(literal.Children);
                    Assert.True(ReferenceEquals(expr, literal.Parent));
                    Assert.Equal("literal", literal.Name);
                }
            }
        }
    }
    [Fact]
    public void Build_Throws_WhenMissingChildWithThrowBehavior()
    {
        TreeBuilder builder = new();
        builder.AddRoot("a")
            .WithChildren(new[] { "b" });

        Assert.Throws<InvalidOperationException>(() => builder.Build(MissingChildBehavior.Throw));
    }
    [Fact]
    public void Build_CreatesMissingChild_WhenMissingChildWithCreateBehavior()
    {
        TreeBuilder builder = new();
        builder.AddRoot("a")
            .WithChildren(new[] { "b" });

        var tree = builder.Build(MissingChildBehavior.CreateEmptyNode);



        Assert.Equal(1, tree.Roots.Count);

        {
            var a = tree.Roots[0];
            Assert.Equal("a", a.Name);
            Assert.Null(a.Parent);
            Assert.Equal(1, a.Children.Count);

            {
                var b = a.Children["b"];
                Assert.Equal("b", b.Name);
                Assert.True(ReferenceEquals(a, b.Parent));
                Assert.Empty(b.Children);
            }
        }
    }
}
