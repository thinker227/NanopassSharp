using System;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Tests;

public class NodePathTests
{
    [Fact]
    public void Ctor_ThrowsIfEmptyEnumerable()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new NodePath(Enumerable.Empty<string>());
        });
    }
    [Fact]
    public void Ctor_SucceedsIfNonEmptyEnumerable()
    {
        _ = new NodePath(new[] { "foo", "bar", "baz" });
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
            new NodePath("foo", "baz")
        };
        yield return new object[]
        {
            new NodePath("foo", "bar", "baz", "boo", "far", "zaz"),
            new NodePath("foo", "baz", "baz", "boo", "far")
        };
    }
    [MemberData(nameof(Parent_ReturnsParentPath_Data))]
    [Theory]
    public void Parent_ReturnsParentPath(NodePath path, NodePath expected)
    {
        var parent = path.Parent;
        Assert.Equal(expected, parent);
    }
    [Fact]
    public void Parent_ThrowsIfRoot()
    {
        NodePath path = new("foo");

        Assert.Throws<InvalidOperationException>(() => path.Parent);
    }
    [Fact]
    public void Parent_ThrowsIfEmpty()
    {
        NodePath path = new();

        Assert.Throws<InvalidOperationException>(() => path.Parent);
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

        Assert.Equal(expected, root);
    }
    [Fact]
    public void Root_ThrowsIfEmpty()
    {
        NodePath path = new();

        Assert.Throws<InvalidOperationException>(() => path.Root);
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

        Assert.Equal(expected, leaf);
    }

    [InlineData(new object[] { new[] { "foo" } })]
    [InlineData(new object[] { new[] { "foo", "bar" } })]
    [InlineData(new object[] { new[] { "foo", "bar", "baz", "boo", "far", "zaz" } })]
    [Theory]
    public void Depth_ReturnsArrayLengthMinusOne(string[] pathValues)
    {
        NodePath path = new(pathValues);

        int expected = pathValues.Length - 1;
        Assert.Equal(expected, path.Depth);
    }
    [Fact]
    public void Depth_ThrowsWhenEmpty()
    {
        NodePath path = new();

        Assert.Throws<InvalidOperationException>(() => path.Depth);
    }

    [Fact]
    public void IsRoot_ReturnsTrue_WhenRoot()
    {
        NodePath path = new("foo");

        Assert.True(path.IsRoot);
    }
    [Fact]
    public void IsRoot_ReturnsFalse_WhenNotRoot()
    {
        NodePath path = new("foo", "bar", "baz");

        Assert.False(path.IsRoot);
    }
    [Fact]
    public void IsRoot_ThrowsWhenEmpty()
    {
        NodePath path = new();

        Assert.Throws<InvalidOperationException>(() => path.IsRoot);
    }

    [InlineData(new[] { "foo" }, new[] { "foo" })]
    [InlineData(new[] { "foo", "bar", "baz" }, new[] { "foo", "bar", "baz" })]
    [Theory]
    public void Equals_ReturnsTrue(string[] a, string[] b)
    {
        NodePath pathA = new(a);
        NodePath pathB = new(b);

        Assert.True(pathA.Equals(pathB));
    }
    [InlineData(new[] { "foo" }, new[] { "foo", "bar" })]
    [InlineData(new[] { "foo", "bar", "baz" }, new[] { "boo", "far", "zaz" })]
    [Theory]
    public void Equals_ReturnsFalse(string[] a, string[] b)
    {
        NodePath pathA = new(a);
        NodePath pathB = new(b);

        Assert.False(pathA.Equals(pathB));
    }

    [InlineData(new object[] { new[] { "foo" } })]
    [InlineData(new object[] { new[] { "foo", "bar", "baz" } })]
    [Theory]
    public void ToString_ReturnsPeriodSeparatedPath(string[] pathValues)
    {
        NodePath path = new(pathValues);

        string expected = string.Join('.', pathValues);
        Assert.Equal(expected, path.ToString());
    }

    [InlineData(new object[] { new[] { "foo" } })]
    [InlineData(new object[] { new[] { "foo", "bar", "baz" } })]
    [Theory]
    public void Enumeration_ReturnsReverseOrder(string[] pathValues)
    {
        NodePath path = new(pathValues);

        var expected = pathValues.Reverse();
        Assert.Equal(expected, path);
    }

    [InlineData("")]
    [InlineData("foo..bar")]
    [InlineData(".foo")]
    [InlineData("foo.bar.")]
    [Theory]
    public void Parse_ReturnsNullIfBadFormat(string str)
    {
        var path = NodePath.Parse(str);

        Assert.Null(path);
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

        Assert.Equal(expected, path);
    }

    [Fact]
    public void CreateLeafPath_ReturnsLeafPath()
    {
        NodePath path = new("foo");
        var leafPath = path.CreateLeafPath("bar");

        NodePath expected = new("foo", "bar");
        Assert.Equal(expected, leafPath);
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
        Assert.Equal(expected, parentPaths);
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
        Assert.Equal(expected, parentPathsAndSelf);
    }
}
