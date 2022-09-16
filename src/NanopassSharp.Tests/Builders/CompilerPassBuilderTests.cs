using System;
using System.Collections.Generic;
using NanopassSharp.Tests;

namespace NanopassSharp.Builders.Tests;

public class CompilerPassBuilderTests
{
    [Fact]
    public void Ctor_SetsProperties()
    {
        CompilerPassBuilder builder = new("foo");

        builder.Name.ShouldBe("foo");
        builder.Documentation.ShouldBeNull();
        builder.Transformations.ShouldNotBeNull();
        builder.Transformations.ShouldBeEmpty();
        builder.Previous.ShouldBeNull();
        builder.Next.ShouldBeNull();
    }

    private static IEnumerable<object[]> FromPass_ReturnsCorrectBuilder_Data()
    {
        yield return new object[]
        {
            new CompilerPass("foo", "docs foo", new PassTransformations(new ITransformationDescription[]
            {
                new MockTransformationDescription(),
                new MockTransformationDescription(),
                new MockTransformationDescription(),
            }), "bar", "baz")
        };
        yield return new object[]
        {
            new CompilerPass("bar", null, new PassTransformations(Array.Empty<ITransformationDescription>()), "bar", null)
        };
    }

    [MemberData(nameof(FromPass_ReturnsCorrectBuilder_Data))]
    [Theory]
    public void FromPass_ReturnsCorrectBuilder(CompilerPass pass)
    {
        var builder = CompilerPassBuilder.FromPass(pass);

        builder.Name.ShouldBe(pass.Name);
        builder.Documentation.ShouldBe(pass.Documentation);
        builder.Transformations.ShouldBe(pass.Transformations.Transformations);
        builder.Previous.ShouldBe(pass.Previous);
        builder.Next.ShouldBe(pass.Next);
    }

    [Fact]
    public void WithDocumentation_SetsDocumentation()
    {
        CompilerPassBuilder builder = new("foo");
        builder.WithDocumentation("docs foo");

        builder.Documentation.ShouldBe("docs foo");
    }

    [Fact]
    public void WithDocumentation_ReturnsSelf()
    {
        CompilerPassBuilder builder = new("foo");
        var withDocumentation = builder.WithDocumentation("docs foo");

        withDocumentation.ShouldBeSameAs(builder);
    }

    [Fact]
    public void WithTransformations_SetsTransformations()
    {
        CompilerPassBuilder builder = new("foo");

        var descriptions = new ITransformationDescription[]
        {
            new MockTransformationDescription(),
            new MockTransformationDescription(),
            new MockTransformationDescription(),
        };
        builder.WithTransformations(descriptions);

        builder.Transformations.ShouldBe(descriptions);
    }

    [Fact]
    public void WithTransformations_ReturnsSelf()
    {
        CompilerPassBuilder builder = new("foo");
        var withTransformations = builder.WithTransformations(Array.Empty<ITransformationDescription>());

        withTransformations.ShouldBeSameAs(builder);
    }

    [Fact]
    public void AddTransformation_AddsTransformation()
    {
        CompilerPassBuilder builder = new("foo");
        MockTransformationDescription transformation = new();
        builder.AddTransformation(transformation);

        var expected = new[]
        {
            transformation
        };
        builder.Transformations.ShouldBe(expected);
    }

    [Fact]
    public void AddTransformation_ReturnsSelf()
    {
        CompilerPassBuilder builder = new("foo");
        var addTransformation = builder.AddTransformation(new MockTransformationDescription());

        addTransformation.ShouldBeSameAs(builder);
    }

    [Fact]
    public void WithPrevious_SetsPrevious()
    {
        CompilerPassBuilder builder = new("foo");
        builder.WithPrevious("bar");

        builder.Previous.ShouldBe("bar");
    }

    [Fact]
    public void WithPrevious_ReturnsSelf()
    {
        CompilerPassBuilder builder = new("foo");
        var withPrevious = builder.WithPrevious("bar");

        withPrevious.ShouldBeSameAs(builder);
    }

    [Fact]
    public void WithNext_SetsNext()
    {
        CompilerPassBuilder builder = new("foo");
        builder.WithNext("bar");

        builder.Next.ShouldBe("bar");
    }

    [Fact]
    public void WithNext_ReturnsSelf()
    {
        CompilerPassBuilder builder = new("foo");
        var withNext = builder.WithNext("bar");

        withNext.ShouldBeSameAs(builder);
    }
}
