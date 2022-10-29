using System;
using System.Collections.Generic;
using NanopassSharp.Builders;

namespace NanopassSharp.Tests;

public class AstNodeTests
{
    private static AstNode[] CreateIdenticalNodes(string name, Action<AstNodeBuilder>? configure = null)
    {
        AstNodeHierarchyBuilder builderA = new();
        var nodeA = builderA.AddRoot(name);

        AstNodeHierarchyBuilder builderB = new();
        var nodeB = builderB.AddRoot(name);

        if (configure is not null)
        {
            configure(nodeA);
            configure(nodeB);
        }

        return new[] { nodeA.Build(), nodeB.Build() };
    }

    private static AstNode CreateNode(string name, Action<AstNodeBuilder>? configure = null)
    {
        AstNodeHierarchyBuilder builder = new();
        var node = builder.AddRoot(name);

        if (configure is not null)
        {
            configure(node);
        }

        return node.Build();
    }

    [Fact]
    public void EmptyNode_EqualsItself()
    {
        var node = CreateNode("foo");
        
        node.Equals(node).ShouldBeTrue();
    }

    private static IEnumerable<object[]> Nodes_AreEqual_Data()
    {
        yield return CreateIdenticalNodes("a");

        yield return CreateIdenticalNodes("foo", node => node
            .WithDocumentation("this is a foo"));

        yield return CreateIdenticalNodes("a", node =>
        {
            var b = node.AddChild("b");
            b.AddChild("c");
            node.AddChild("d");
        });

        yield return CreateIdenticalNodes("foo", node =>
        {
            node.AddMember("a");
            node.AddMember("b");
            node.AddMember("c");
        });

        yield return CreateIdenticalNodes("foo", node =>
        {
            node.AddAttribute("attribute");
            node.AddAttribute(101);
            node.AddAttribute(true);
        });
    }

    [MemberData(nameof(Nodes_AreEqual_Data))]
    [Theory]
    public void Nodes_AreEqual(AstNode a, AstNode b) =>
        a.Equals(b).ShouldBeTrue();

    private static IEnumerable<object[]> Nodes_AreNotEqual_Data()
    {
        yield return new[]
        {
            CreateNode("a"),
            CreateNode("b")
        };

        yield return new[]
        {
            CreateNode("foo", node => node
                .WithDocumentation("this is a foo")),
            CreateNode("foo")
        };

        yield return new[]
        {
            CreateNode("foo", node => node
                .WithDocumentation("this is a foo")),
            CreateNode("foo", node => node
                .WithDocumentation("this is not a foo"))
        };

        yield return new[]
        {
            CreateNode("a", node =>
            {
                var b = node.AddChild("b");
                b.AddChild("c");
                node.AddChild("d");
            }),
            CreateNode("a")
        };

        yield return new[]
        {
            CreateNode("a", node =>
            {
                var b = node.AddChild("b");
                b.AddChild("c");
                node.AddChild("d");
            }),
            CreateNode("a", node =>
            {
                node.AddChild("b");
            })
        };

        yield return new[]
        {
            CreateNode("foo", node =>
            {
                node.AddMember("a");
                node.AddMember("b");
                node.AddMember("c");
            }),
            CreateNode("foo")
        };

        yield return new[]
        {
            CreateNode("foo", node =>
            {
                node.AddMember("a");
                node.AddMember("b");
                node.AddMember("c");
            }),
            CreateNode("foo", node =>
            {
                node.AddMember("a");
                node.AddMember("z");
            })
        };

        yield return new[]
        {
            CreateNode("foo", node =>
            {
                node.AddAttribute("attribute");
                node.AddAttribute(101);
                node.AddAttribute(true);
            }),
            CreateNode("foo")
        };

        yield return new[]
        {
            CreateNode("foo", node =>
            {
                node.AddAttribute("attribute");
                node.AddAttribute(101);
                node.AddAttribute(true);
            }),
            CreateNode("foo", node =>
            {
                node.AddAttribute("attribute");
                node.AddAttribute(false);
            })
        };
    }

    [MemberData(nameof(Nodes_AreNotEqual_Data))]
    [Theory]
    public void Nodes_AreNotEqual(AstNode a, AstNode b) =>
        a.Equals(b).ShouldBeFalse();
}
