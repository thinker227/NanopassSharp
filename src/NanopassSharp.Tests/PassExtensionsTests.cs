using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NanopassSharp.Builders;

namespace NanopassSharp.Tests;

public class PassExtensionsTests
{
    private static IEnumerable<object[]> GetNodes_GetsAllNodes_Data()
    {
        {
            yield return new object[]
            {
                AstNodeHierarchy.Empty,
                Array.Empty<string>()
            };
        }

        {
            AstNodeHierarchyBuilder builder = new();
            builder.AddRoot("a");

            yield return new object[]
            {
                builder.Build(),
                new[]
                {
                    "a"
                }
            };
        }

        {
            AstNodeHierarchyBuilder builder = new();
            var a = builder.AddRoot("a");
            var b = a.AddChild("b");
            b.AddChild("c");
            b.AddChild("d");
            a.AddChild("e");

            yield return new object[]
            {
                builder.Build(),
                new[]
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e"
                }
            };
        }
    }

    [MemberData(nameof(GetNodes_GetsAllNodes_Data))]
    [Theory]
    public void GetNodes_GetsAllNodes(AstNodeHierarchy tree, IEnumerable<string> expectedNames)
    {
        var nodes = tree.GetNodes();

        nodes.Select(n => n.Name).ShouldBe(expectedNames);
    }

    private static IEnumerable<object[]> GetDecendantNodes_GetAllDecendantNodes_Data(bool includeSelf)
    {
        {
            AstNodeHierarchyBuilder builder = new();
            builder.AddRoot("a");

            string[] names = new[]
            {
                "a"
            };

            yield return new object[]
            {
                builder.Build().Roots[0],
                includeSelf ? names : names.Skip(1)
            };
        }

        {
            AstNodeHierarchyBuilder builder = new();
            var a = builder.AddRoot("a");
            var b = a.AddChild("b");
            b.AddChild("c");
            b.AddChild("d");
            a.AddChild("e");

            string[] names = new[]
            {
                "a",
                "b",
                "c",
                "d",
                "e"
            };

            yield return new object[]
            {
                builder.Build().Roots[0],
                includeSelf ? names : names.Skip(1)
            };
        }
    }

    [MemberData(nameof(GetDecendantNodes_GetAllDecendantNodes_Data), false)]
    [Theory]
    public void GetDecendantNodes_GetAllDecendantNodes(AstNode node, IEnumerable<string> expectedNames)
    {
        var decendants = node.GetDecendantNodes();

        decendants.Select(n => n.Name).ShouldBe(expectedNames);
    }

    [MemberData(nameof(GetDecendantNodes_GetAllDecendantNodes_Data), true)]
    [Theory]
    public void GetDecendantNodesAndSelf_GetAllDecendantNodesAndSelf(AstNode node, IEnumerable<string> expectedNames)
    {
        var decendants = node.GetDecendantNodesAndSelf();

        decendants.Select(n => n.Name).ShouldBe(expectedNames);
    }

    private static IEnumerable<object[]> GetRoot_ReturnsRoot_Data()
    {
        {
            AstNodeHierarchyBuilder builder = new();
            var foo = builder.AddRoot("foo");

            yield return new object[]
            {
                foo.Build(),
                "foo"
            };
        }

        {
            AstNodeHierarchyBuilder builder = new();
            var a = builder.AddRoot("a");
            var b = a.AddChild("b");
            var c = b.AddChild("c");

            yield return new object[]
            {
                c.Build(),
                "a"
            };
        }
    }

    [MemberData(nameof(GetRoot_ReturnsRoot_Data))]
    [Theory]
    public void GetRoot_ReturnsRoot(AstNode node, string expectedRootName)
    {
        var root = node.GetRoot();

        root.Name.ShouldBe(expectedRootName);
    }

    private static IEnumerable<object[]> GetPath_ReturnsPath_Data()
    {
        {
            AstNodeHierarchyBuilder builder = new();
            var foo = builder.AddRoot("foo");

            yield return new object[]
            {
                foo.Build(),
                NodePath.ParseUnsafe("foo")
            };
        }

        {
            AstNodeHierarchyBuilder builder = new();
            var a = builder.AddRoot("a");
            var b = a.AddChild("b");
            var c = b.AddChild("c");
            var d = c.AddChild("d");
            var e = d.AddChild("e");

            yield return new object[]
            {
                e.Build(),
                NodePath.ParseUnsafe("a.b.c.d.e")
            };
        }
    }

    [MemberData(nameof(GetPath_ReturnsPath_Data))]
    [Theory]
    public void GetPath_ReturnsPath(AstNode node, NodePath expectedPath)
    {
        var path = node.GetPath();

        path.ShouldBe(expectedPath);
    }

    private static IEnumerable<object?[]> GetNodeFromPath_ReturnsNode_Data()
    {
        {
            AstNodeHierarchyBuilder builder = new();
            builder.AddRoot("foo");

            yield return new object[]
            {
                builder.Build(),
                NodePath.ParseUnsafe("foo"),
                "foo"
            };
        }

        {
            AstNodeHierarchyBuilder builder = new();
            var a = builder.AddRoot("a");
            var b = a.AddChild("b");
            var c = b.AddChild("c");
            var d = c.AddChild("d");
            d.AddChild("e");

            yield return new object[]
            {
                builder.Build(),
                NodePath.ParseUnsafe("a.b.c.d.e"),
                "e"
            };
        }

        {
            AstNodeHierarchyBuilder builder = new();
            var a = builder.AddRoot("a");
            var b = a.AddChild("b");
            b.AddChild("c");

            yield return new object?[]
            {
                builder.Build(),
                NodePath.ParseUnsafe("a.b.d.e"),
                null
            };
        }

        {
            AstNodeHierarchyBuilder builder = new();
            builder.AddRoot("a");

            yield return new object?[]
            {
                builder.Build(),
                NodePath.ParseUnsafe("b"),
                null
            };
        }
    }

    [MemberData(nameof(GetNodeFromPath_ReturnsNode_Data))]
    [Theory]
    public void GetNodeFromPath_ReturnsNode(AstNodeHierarchy tree, NodePath path, string? expectedName)
    {
        var node = tree.GetNodeFromPath(path);

        if (expectedName is null)
        {
            node.ShouldBeNull();
        }
        else
        {
            node.ShouldNotBeNull();
            node.Name.ShouldBe(expectedName);
        }
    }
}
