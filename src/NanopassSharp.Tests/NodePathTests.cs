using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
        new NodePath(new[] { "foo", "bar", "baz" });
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenRoot()
    {
        NodePath a = new("foo");
        NodePath b = new("foo");

        Assert.True(a.Equals(b));
    }
    [Fact]
    public void Equals_ReturnsFalse_WhenDifferentDepth()
    {
        NodePath a = new("foo");
        NodePath b = new(new[] { "foo", "bar" });

        Assert.False(a.Equals(b));
    }
    [Fact]
    public void Equals_ReturnsTrue_WhenSameElements()
    {
        NodePath a = new(new[] { "foo", "bar", "baz" });
        NodePath b = new(new[] { "foo", "bar", "baz" });

        Assert.True(a.Equals(b));
    }
    [Fact]
    public void Equals_ReturnsFalse_WhenDifferentElements()
    {
        NodePath a = new(new[] { "foo", "bar", "baz" });
        NodePath b = new(new[] { "boo", "far", "zaz" });

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void ToString_ReturnsSingleNameForRoot()
    {
        NodePath path = new("foo");

        Assert.Equal("foo", path.ToString());
    }
    [Fact]
    public void ToString_ReturnsPeriodSeparatedForMultipleElements()
    {
        NodePath path = new(new[] { "foo", "bar", "baz" });

        Assert.Equal("foo.bar.baz", path.ToString());
    }

    [Fact]
    public void Enumeration_ReturnsCorrectOrder()
    {
        NodePath path = new(new[] { "foo", "bar", "baz" });

        string[] expected = new[] { "baz", "bar", "foo" };
        Assert.Equal(expected, path);
    }

    [Fact]
    public void Parse_ReturnsNullIfEmptyString()
    {
        var path = NodePath.Parse("");

        Assert.Null(path);
    }
    [Fact]
    public void Parse_ReturnsPathIfRoot()
    {
        var path = NodePath.Parse("foo");

        NodePath expected = new(new[] { "foo" });
        Assert.Equal(expected, path);
    }
    [Fact]
    public void Parse_ReturnsPathIfPeriodSeparatedNames()
    {
        var path = NodePath.Parse("foo.bar.baz");

        NodePath expected = new(new[] { "foo", "bar", "baz" });
        Assert.Equal(expected, path);
    }
    [Fact]
    public void Parse_ReturnsNullIfMissingName()
    {
        var path = NodePath.Parse("foo..bar");

        Assert.Null(path);
    }

    [Fact]
    public void CreateLeafPath_ReturnsLeafPath()
    {
        NodePath path = new("foo");
        var leafPath = path.CreateLeafPath("bar");

        NodePath expected = new(new[] { "foo", "bar" });
        Assert.Equal(expected, leafPath);
    }

    [Fact]
    public void GetParentPaths_ReturnsParentPaths()
    {
        NodePath path = new(new[] { "foo", "bar", "baz" });

        var expected = new NodePath[]
        {
            new NodePath(new[] { "foo", "bar" }),
            new NodePath(new[] { "foo" })
        };
        Assert.Equal(expected, path.GetParentPaths());
    }
    [Fact]
    public void GetParentPaths_ReturnsEmptyForRoot()
    {
        NodePath path = new("foo");

        Assert.Empty(path.GetParentPaths());
    }

    [Fact]
    public void GetParentPathsAndSelf_ReturnsParentPaths()
    {
        NodePath path = new(new[] { "foo", "bar", "baz" });

        var expected = new NodePath[]
        {
            new NodePath(new[] { "foo", "bar", "baz" }),
            new NodePath(new[] { "foo", "bar" }),
            new NodePath(new[] { "foo" })
        };
        Assert.Equal(expected, path.GetParentPathsAndSelf());
    }
    [Fact]
    public void GetParentPathsAndSelf_ReturnsSelfForRoot()
    {
        NodePath path = new("foo");

        var expected = new[] { path };
        Assert.Equal(expected, path.GetParentPathsAndSelf());
    }
}
