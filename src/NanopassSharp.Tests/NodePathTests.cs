using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Tests;

public class NodePathTests
{
    [Fact]
    public void Ctor_ThrowsIfEmptyEnumerable()
    {
        Should.Throw<ArgumentException>(() =>
        {
            new NodePath(Enumerable.Empty<string>());
        });
    }

    [Fact]
    public void Ctor_SucceedsIfNonEmptyEnumerable()
    {
        Should.NotThrow(() => new NodePath(new[] { "foo", "bar", "baz" }));
    }

    private static IEnumerable<object[]> Parent_ReturnsParentPath_Data()
    {
        yield return new object[]
        {
            new NodePath("foo", "bar"),
            new NodePath("foo")
        };
        yield return new object[]
        {
            new NodePath("foo", "bar", "baz"),
            new NodePath("foo", "bar")
        };
        yield return new object[]
        {
            new NodePath("foo", "bar", "baz", "boo", "far", "zaz"),
            new NodePath("foo", "bar", "baz", "boo", "far")
        };
    }

    [MemberData(nameof(Parent_ReturnsParentPath_Data))]
    [Theory]
    public void Parent_ReturnsParentPath(NodePath path, NodePath expected)
    {
        var parent = path.Parent;
        parent.ShouldBe(expected);
    }

    [Fact]
    public void Parent_ThrowsIfRoot()
    {
        NodePath path = new("foo");

        Should.Throw<InvalidOperationException>(() => path.Parent);
    }

    [Fact]
    public void Parent_ThrowsIfEmpty()
    {
        NodePath path = new();

        Should.Throw<InvalidOperationException>(() => path.Parent);
    }

    private static IEnumerable<object[]> Root_ReturnsRoot_Data()
    {
        yield return new object[]
        {
            new NodePath("foo"),
            "foo"
        };
        yield return new object[]
        {
            new NodePath("baz", "bar"),
            "baz"
        };
        yield return new object[]
        {
            new NodePath("foo", "bar", "baz", "boo", "far", "zaz"),
            "foo"
        };
    }

    [MemberData(nameof(Root_ReturnsRoot_Data))]
    [Theory]
    public void Root_ReturnsRoot(NodePath path, string expected)
    {
        string root = path.Root;

        root.ShouldBe(expected);
    }

    [Fact]
    public void Root_ThrowsIfEmpty()
    {
        NodePath path = new();

        Should.Throw<InvalidOperationException>(() => path.Root);
    }

    private static IEnumerable<object[]> Leaf_ReturnsLeaf_Data()
    {
        yield return new object[]
        {
            new NodePath("foo"),
            "foo"
        };
        yield return new object[]
        {
            new NodePath("foo", "bar", "baz"),
            "baz"
        };
        yield return new object[]
        {
            new NodePath("foo", "bar", "baz", "boo", "far", "zaz"),
            "zaz"
        };
    }

    [MemberData(nameof(Leaf_ReturnsLeaf_Data))]
    [Theory]
    public void Leaf_ReturnsLeaf(NodePath path, string expected)
    {
        string leaf = path.Leaf;

        leaf.ShouldBe(expected);
    }

    [InlineData(new object[] { new[] { "foo" } })]
    [InlineData(new object[] { new[] { "foo", "bar" } })]
    [InlineData(new object[] { new[] { "foo", "bar", "baz", "boo", "far", "zaz" } })]
    [Theory]
    public void Depth_ReturnsArrayLengthMinusOne(string[] pathValues)
    {
        NodePath path = new(pathValues);

        int expected = pathValues.Length - 1;
        path.Depth.ShouldBe(expected);
    }

    [Fact]
    public void Depth_ThrowsWhenEmpty()
    {
        NodePath path = new();

        Should.Throw<InvalidOperationException>(() => path.Depth);
    }

    [Fact]
    public void IsRoot_ReturnsTrue_WhenRoot()
    {
        NodePath path = new("foo");

        path.IsRoot.ShouldBeTrue();
    }

    [Fact]
    public void IsRoot_ReturnsFalse_WhenNotRoot()
    {
        NodePath path = new("foo", "bar", "baz");

        path.IsRoot.ShouldBeFalse();
    }

    [Fact]
    public void IsRoot_ThrowsWhenEmpty()
    {
        NodePath path = new();

        Should.Throw<InvalidOperationException>(() => path.IsRoot);
    }

    [InlineData(new[] { "foo" }, new[] { "foo" })]
    [InlineData(new[] { "foo", "bar", "baz" }, new[] { "foo", "bar", "baz" })]
    [Theory]
    public void Equals_ReturnsTrue(string[] a, string[] b)
    {
        NodePath pathA = new(a);
        NodePath pathB = new(b);

        pathA.Equals(pathB).ShouldBeTrue();
    }

    [InlineData(new[] { "foo" }, new[] { "foo", "bar" })]
    [InlineData(new[] { "foo", "bar", "baz" }, new[] { "boo", "far", "zaz" })]
    [Theory]
    public void Equals_ReturnsFalse(string[] a, string[] b)
    {
        NodePath pathA = new(a);
        NodePath pathB = new(b);

        pathA.Equals(pathB).ShouldBeFalse();
    }

    [InlineData(new object[] { new[] { "foo" } })]
    [InlineData(new object[] { new[] { "foo", "bar", "baz" } })]
    [Theory]
    public void ToString_ReturnsPeriodSeparatedPath(string[] pathValues)
    {
        NodePath path = new(pathValues);

        string expected = string.Join('.', pathValues);
        path.ToString().ShouldBe(expected);
    }

    [InlineData("")]
    [InlineData("foo..bar")]
    [InlineData(".foo")]
    [InlineData("foo.bar.")]
    [Theory]
    public void Parse_ReturnsNullIfBadFormat(string str)
    {
        var path = NodePath.Parse(str);

        path.ShouldBeNull();
    }

    private static IEnumerable<object[]> Parse_ReturnsExpected_Data()
    {
        yield return new object[] { "foo", new NodePath("foo") };
        yield return new object[] { "foo.bar", new NodePath("foo", "bar") };
        yield return new object[] { "foo.bar.baz.boo.far.zaz", new NodePath("foo", "bar", "baz", "boo", "far", "zaz") };
    }

    [MemberData(nameof(Parse_ReturnsExpected_Data))]
    [Theory]
    public void Parse_ReturnsExpected(string str, NodePath expected)
    {
        var path = NodePath.Parse(str);

        path.ShouldBe(expected);
    }

    [Fact]
    public void CreateLeafPath_ReturnsLeafPath()
    {
        NodePath path = new("foo");
        var leafPath = path.CreateLeafPath("bar");

        NodePath expected = new("foo", "bar");
        leafPath.ShouldBe(expected);
    }

    private static IEnumerable<object[]> GetParentPaths_ReturnsParentPaths_Data()
    {
        yield return new object[]
        {
            new NodePath("foo"),
            Array.Empty<NodePath>()
        };
        yield return new object[]
        {
            new NodePath("foo", "bar", "baz"),
            new NodePath[]
            {
                new("foo", "bar"),
                new("foo")
            }
        };
    }

    [MemberData(nameof(GetParentPaths_ReturnsParentPaths_Data))]
    [Theory]
    public void GetParentPaths_ReturnsParentPaths(NodePath path, NodePath[] expected)
    {
        var parentPaths = path.GetParentPaths();
        parentPaths.ShouldBe(expected);
    }

    private static IEnumerable<object[]> GetParentPathsAndSelf_ReturnsParentPathsAndSelf_Data()
    {
        yield return new object[]
        {
            new NodePath("foo"),
            new NodePath[]
            {
                new("foo")
            }
        };
        yield return new object[]
        {
            new NodePath("foo", "bar", "baz"),
            new NodePath[]
            {
                new("foo", "bar", "baz"),
                new("foo", "bar"),
                new("foo")
            }
        };
    }

    [MemberData(nameof(GetParentPathsAndSelf_ReturnsParentPathsAndSelf_Data))]
    [Theory]
    public void GetParentPathsAndSelf_ReturnsParentPathsAndSelf(NodePath path, NodePath[] expected)
    {
        var parentPathsAndSelf = path.GetParentPathsAndSelf();
        parentPathsAndSelf.ShouldBe(expected);
    }

    [InlineData(new object[] { new[] { "foo" } })]
    [InlineData(new object[] { new[] { "foo", "bar", "baz" } })]
    [InlineData(new object[] { new[] { "foo", "bar", "baz", "boo", "far", "zaz" } })]
    [Theory]
    public void GetNodes_ReturnsNodesInOrder(string[] pathValues)
    {
        NodePath path = new(pathValues);
        var nodes = path.GetNodes();

        string[] expected = pathValues;
        nodes.ShouldBe(expected);
    }
}
