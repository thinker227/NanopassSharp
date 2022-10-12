using System;
using System.Collections.Generic;
using NanopassSharp.Builders;

namespace NanopassSharp.Tests;

public class AstNodeMemberTests
{
    private static AstNodeMember[] CreateIdenticalMembers(string name, Action<AstNodeMemberBuilder>? configure = null)
    {
        AstNodeMemberBuilder a = new(name);
        AstNodeMemberBuilder b = new(name);

        if (configure is not null)
        {
            configure(a);
            configure(b);
        }

        return new[] { a.Build(), b.Build() };
    }

    private static AstNodeMember CreateMember(string name, Action<AstNodeMemberBuilder>? configure = null)
    {
        AstNodeMemberBuilder builder = new(name);

        if (configure is not null)
        {
            configure(builder);
        }

        return builder.Build();
    }

    private static IEnumerable<object[]> Members_AreEqual_Data()
    {
        yield return CreateIdenticalMembers("foo");

        yield return CreateIdenticalMembers("foo", member => member
            .WithDocumentation("this is a foo"));

        yield return CreateIdenticalMembers("foo", member => member
            .WithType("string"));

        yield return CreateIdenticalMembers("foo", member => member
            .AddAttribute("attribute")
            .AddAttribute(101)
            .AddAttribute(true));
    }

    [MemberData(nameof(Members_AreEqual_Data))]
    [Theory]
    public void Members_AreEqual(AstNodeMember a, AstNodeMember b) =>
        a.Equals(b).ShouldBeTrue();

    private static IEnumerable<object[]> Members_AreNotEqual_Data()
    {
        yield return new[]
        {
            CreateMember("a"),
            CreateMember("b")
        };

        yield return new[]
        {
            CreateMember("foo", member => member
                .WithDocumentation("this is a foo")),
            CreateMember("foo")
        };

        yield return new[]
        {
            CreateMember("foo", member => member
                .WithType("string")),
            CreateMember("foo")
        };

        yield return new[]
        {
            CreateMember("foo", member => member
                .AddAttribute("attribute")
                .AddAttribute(101)
                .AddAttribute(true)),
            CreateMember("foo")
        };

        yield return new[]
        {
            CreateMember("foo", member => member
                .AddAttribute("attribute")
                .AddAttribute(101)
                .AddAttribute(true)),
            CreateMember("foo")
        };

        yield return new[]
        {
            CreateMember("foo", member => member
                .AddAttribute("attribute")
                .AddAttribute(101)
                .AddAttribute(true)),
            CreateMember("foo", member => member
                .AddAttribute("attribute")
                .AddAttribute(false))
        };
    }

    [MemberData(nameof(Members_AreNotEqual_Data))]
    [Theory]
    public void Members_AreNotEqual(AstNodeMember a, AstNodeMember b) =>
        a.Equals(b).ShouldBeFalse();
}
