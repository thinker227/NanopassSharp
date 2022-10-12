using System;
using System.Collections.Generic;
using NanopassSharp.Builders;

namespace NanopassSharp.Tests;

public class AstNodeHierarchyTests
{
    private static AstNodeHierarchy[] CreateIdenticalHierarchies(Action<AstNodeHierarchyBuilder>? configure = null)
    {
        AstNodeHierarchyBuilder a = new();
        AstNodeHierarchyBuilder b = new();

        if (configure is not null)
        {
            configure(a);
            configure(b);
        }

        return new[] { a.Build(), b.Build() };
    }

    private static AstNodeHierarchy CreateHierarchy(Action<AstNodeHierarchyBuilder>? configure = null)
    {
        AstNodeHierarchyBuilder builder = new();
        
        if (configure is not null)
        {
            configure(builder);
        }

        return builder.Build();
    }

    [Fact]
    public void EmptyHierarchy_IsEmpty()
    {
        var hierarchy = AstNodeHierarchy.Empty;

        hierarchy.Roots.ShouldBeEmpty();
    }

    private static IEnumerable<object[]> Hierarchies_AreEqual_Data()
    {
        yield return CreateIdenticalHierarchies();

        yield return CreateIdenticalHierarchies(hierarchy =>
        {
            hierarchy.AddRoot("a");
        });

        yield return CreateIdenticalHierarchies(hierarchy =>
        {
            var a = hierarchy.AddRoot("a");
            var b = a.AddChild("b");
            b.AddChild("c");
            a.AddChild("d");
            var e = hierarchy.AddRoot("e");
            e.AddChild("f");
        });
    }

    [MemberData(nameof(Hierarchies_AreEqual_Data))]
    [Theory]
    public void Hierarchies_AreEqual(AstNodeHierarchy a, AstNodeHierarchy b) =>
        a.Equals(b).ShouldBeTrue();

    private static IEnumerable<object[]> Hierarchies_AreNotEqual_Data()
    {
        yield return new[]
        {
            CreateHierarchy(),
            CreateHierarchy(hierarchy =>
            {
                hierarchy.AddRoot("a");
            })
        };

        yield return new[]
        {
            CreateHierarchy(hierarchy =>
            {
                hierarchy.AddRoot("a");
                hierarchy.AddRoot("b");
                hierarchy.AddRoot("c");
            }),
            CreateHierarchy(hierarchy =>
            {
                hierarchy.AddRoot("a");
            })
        };

        yield return new[]
        {
            CreateHierarchy(hierarchy =>
            {
                var a = hierarchy.AddRoot("a");
                var b = a.AddChild("b");
                b.AddChild("c");
                a.AddChild("d");
                var e = hierarchy.AddRoot("e");
                e.AddChild("f");
            }),
            CreateHierarchy(hierarchy =>
            {
                hierarchy.AddRoot("a");
                hierarchy.AddRoot("e");
            })
        };
    }

    [MemberData(nameof(Hierarchies_AreNotEqual_Data))]
    [Theory]
    public void Hierarchies_AreNotEqual(AstNodeHierarchy a, AstNodeHierarchy b) =>
        a.Equals(b).ShouldBeFalse();
}
