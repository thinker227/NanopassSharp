using System.Collections.Generic;

namespace NanopassSharp.Builders.Tests;

public class AstNodeMemberBuilderTests
{
    [Fact]
    public void Ctor_SetsProperties()
    {
        AstNodeMemberBuilder builder = new("a");

        builder.Name.ShouldBe("a");
        builder.Documentation.ShouldBeNull();
        builder.Type.ShouldBeNull();
        builder.Attributes.ShouldNotBeNull();
        builder.Attributes.ShouldBeEmpty();
    }

    private static IEnumerable<object[]> FromMember_ReturnsCorrectBuilder_Data()
    {
        yield return new object[]
        {
            new AstNodeMember(
                "a",
                "docs a",
                "type a",
                new HashSet<object>(new object[] { "attribute a", 0, true })
            )
        };
        yield return new object[]
        {
            new AstNodeMember(
                "b",
                "docs b",
                "type b",
                new HashSet<object>(new object[] { "attribute b", 1, false })
            )
        };
        yield return new object[]
        {
            new AstNodeMember(
                "c",
                "docs c",
                "type c",
                new HashSet<object>(new object[] { "attribute c", 2, true })
            )
        };
    }

    [MemberData(nameof(FromMember_ReturnsCorrectBuilder_Data))]
    [Theory]
    public void FromMember_ReturnsCorrectBuilder(AstNodeMember member)
    {
        var builder = AstNodeMemberBuilder.FromMember(member);

        builder.Name.ShouldBe(member.Name);
        builder.Documentation.ShouldBe(member.Documentation);
        builder.Type.ShouldBe(member.Type);
        builder.Attributes.ShouldBeSubsetOf(member.Attributes);
    }

    [Fact]
    public void WithName_SetsName()
    {
        AstNodeMemberBuilder builder = new("a");
        builder.WithName("b");

        builder.Name.ShouldBe("b");
    }

    [Fact]
    public void WithName_ReturnsSelf()
    {
        AstNodeMemberBuilder builder = new("a");
        var withName = builder.WithName("b");

        withName.ShouldBeSameAs(builder);
    }

    [Fact]
    public void WithDocumentation_SetsDocumentation()
    {
        AstNodeMemberBuilder builder = new("a");
        builder.WithDocumentation("docs a");

        builder.Documentation.ShouldBe("docs a");
    }

    [Fact]
    public void WithDocumentation_ReturnsSelf()
    {
        AstNodeMemberBuilder builder = new("a");
        var withDocumentation = builder.WithDocumentation("docs a");

        withDocumentation.ShouldBeSameAs(builder);
    }

    [Fact]
    public void WithType_SetsType()
    {
        AstNodeMemberBuilder builder = new("a");
        builder.WithType("type a");

        builder.Type.ShouldBe("type a");
    }

    [Fact]
    public void WithType_ReturnsSelf()
    {
        AstNodeMemberBuilder builder = new("a");
        var withName = builder.WithType("type a");

        withName.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddAttribute_AddsAttribute()
    {
        AstNodeMemberBuilder builder = new("a");
        builder.AddAttribute("attribute");
        builder.AddAttribute(56);
        builder.AddAttribute(true);

        object[] expected = new object[]
        {
            "attribute",
            56,
            true
        };
        builder.Attributes.ShouldBeSubsetOf(expected);
    }

    [Fact]
    public void AddAttribute_ReturnsSelf()
    {
        AstNodeMemberBuilder builder = new("a");
        var addAttribute = builder.AddAttribute("attribute");

        addAttribute.ShouldBeSameAs(builder);
    }

    [Fact]
    public void WithAttributes_SetsAttributes()
    {
        object[] attributes = new object[]
        {
            "attribute",
            56,
            true
        };
        AstNodeMemberBuilder builder = new("a");
        builder.WithAttributes(new HashSet<object>(attributes));

        builder.Attributes.ShouldBeSubsetOf(attributes);
    }

    [Fact]
    public void WithAttributes_ReturnsSelf()
    {
        object[] attributes = new object[]
        {
            "attribute",
            56,
            true
        };
        AstNodeMemberBuilder builder = new("a");
        var withAttributes = builder.WithAttributes(new HashSet<object>(attributes));

        withAttributes.ShouldBeSameAs(builder);
    }

    private static IEnumerable<object[]> Build_ReturnsCorrectMember_Data()
    {
        yield return new object[]
        {
            new AstNodeMemberBuilder("a")
            {
                Documentation = "docs a",
                Type = "type a",
                Attributes = new HashSet<object>(new object[] { "attribute a", 0, true })
            }
        };
        yield return new object[]
        {
            new AstNodeMemberBuilder("b")
            {
                Documentation = "docs b",
                Type = "type b",
                Attributes = new HashSet<object>(new object[] { "attribute b", 1, false })
            }
        };
        yield return new object[]
        {
            new AstNodeMemberBuilder("c")
            {
                Documentation = "docs c",
                Type = "type c",
                Attributes = new HashSet<object>(new object[] { "attribute c", 2, true })
            }
        };
    }

    [MemberData(nameof(Build_ReturnsCorrectMember_Data))]
    [Theory]
    public void Build_ReturnsCorrectMember(AstNodeMemberBuilder builder)
    {
        var member = builder.Build();

        member.Name.ShouldBe(builder.Name);
        member.Documentation.ShouldBe(builder.Documentation);
        member.Type.ShouldBe(builder.Type);
        member.Attributes.ShouldBeSubsetOf(builder.Attributes);
    }
}
