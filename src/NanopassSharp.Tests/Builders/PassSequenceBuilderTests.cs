using System;
using NanopassSharp.Tests;

namespace NanopassSharp.Builders.Tests;

public class PassSequenceBuilderTests
{
    [Fact]
    public void AddPass_ReturnsCorrectPass()
    {
        PassSequenceBuilder builder = new();
        var pass = builder.AddPass("a");

        pass.Name.ShouldBe("a");
        pass.Documentation.ShouldBeNull();
        pass.Transformations.ShouldNotBeNull();
        pass.Transformations.ShouldBeEmpty();
        pass.Previous.ShouldBeNull();
        pass.Next.ShouldBeNull();
    }

    [Fact]
    public void AddPass_CopiesPass()
    {
        PassSequenceBuilder builder = new();

        var transformations = new[]
        {
            new MockTransformationDescription(),
            new MockTransformationDescription(),
            new MockTransformationDescription()
        };
        CompilerPass existingPass = new(
            "a",
            "docs a",
            new PassTransformations(transformations),
            "b",
            "c"
        );
        var pass = builder.AddPass(existingPass);

        pass.Name.ShouldBe("a");
        pass.Documentation.ShouldBe("docs a");
        pass.Transformations.ShouldBe(transformations);
        pass.Previous.ShouldBe("b");
        pass.Next.ShouldBe("c");
    }

    [InlineData("<empty>")]
    [InlineData("<null>")]
    [Theory]
    public void AddPass_Throws_WhenInvalidName(string name)
    {
        PassSequenceBuilder builder = new();

        Should.Throw<ArgumentException>(() => builder.AddPass(name));
    }

    [Fact]
    public void AddPass_ReturnsExistingPass()
    {
        PassSequenceBuilder builder = new();
        var pass1 = builder.AddPass("a");
        var pass2 = builder.AddPass("a");

        pass2.ShouldBeSameAs(pass1);
    }

    [Fact]
    public void SetRoot_ReturnsCorrectPass()
    {
        PassSequenceBuilder builder = new();
        var root = builder.SetRoot("a");

        root.Name.ShouldBe("a");
        root.Documentation.ShouldBeNull();
        root.Transformations.ShouldNotBeNull();
        root.Transformations.ShouldBeEmpty();
        root.Previous.ShouldBeNull();
        root.Next.ShouldBeNull();
    }

    [Fact]
    public void SetRoot_CopiesPass()
    {
        PassSequenceBuilder builder = new();

        var transformations = new[]
        {
            new MockTransformationDescription(),
            new MockTransformationDescription(),
            new MockTransformationDescription()
        };
        CompilerPass pass = new(
            "a",
            "docs a",
            new PassTransformations(transformations),
            "b",
            "c"
        );
        var root = builder.SetRoot(pass);

        root.Name.ShouldBe("a");
        root.Documentation.ShouldBe("docs a");
        root.Transformations.ShouldBe(transformations);
        root.Previous.ShouldBeNull();
        root.Next.ShouldBe("c");
    }

    [InlineData("<empty>")]
    [InlineData("<null>")]
    [Theory]
    public void AddPass_ThrowsWhenInvalidName(string name)
    {
        PassSequenceBuilder builder = new();

        Should.Throw<ArgumentException>(() => builder.SetRoot(name));
    }

    [Fact]
    public void AddPass_ReturnsExistingRoot()
    {
        PassSequenceBuilder builder = new();
        var root1 = builder.SetRoot("a");
        var root2 = builder.SetRoot("a");

        root2.ShouldBeSameAs(root1);
    }

    [Fact]
    public void Build_ReturnsCorrectSequence()
    {
        PassSequenceBuilder builder = new();
        builder.Root = "a";
        builder.AddPass("a")
            .WithNext("b");
        builder.AddPass("b")
            .WithPrevious("a")
            .WithNext("c");
        builder.AddPass("c")
            .WithPrevious("b");
        var sequence = builder.Build();

        var expected = new CompilerPass[]
        {
            new(
                "<empty>",
                null,
                new PassTransformations(Array.Empty<ITransformationDescription>()),
                "<empty>",
                "a"
            ),
            new(
                "a",
                null,
                new PassTransformations(Array.Empty<ITransformationDescription>()),
                "<empty>",
                "b"
            ),
            new(
                "b",
                null,
                new PassTransformations(Array.Empty<ITransformationDescription>()),
                "a",
                "c"
            ),
            new(
                "c",
                null,
                new PassTransformations(Array.Empty<ITransformationDescription>()),
                "b",
                null
            ),
        };
        sequence.ShouldBe(expected);
    }

    [Fact]
    public void Build_Throws_WhenInconsistentLineage()
    {
        PassSequenceBuilder builder = new();
        builder.Root = "a";
        builder.AddPass("a")
            .WithNext("b");
        builder.AddPass("b")
            .WithPrevious("c");

        Should.Throw<InvalidOperationException>(builder.Build);
    }

    [Fact]
    public void Build_DoesNotThrow_WhenNullPrevious()
    {
        PassSequenceBuilder builder = new();
        builder.Root = "a";
        builder.AddPass("a")
            .WithNext("b");
        builder.AddPass("b");

        Should.NotThrow(builder.Build);
    }

    [Fact]
    public void Build_Throws_WhenCircularReference()
    {
        PassSequenceBuilder builder = new();
        builder.Root = "a";
        builder.AddPass("a")
            .WithNext("b");
        builder.AddPass("b")
            .WithNext("c");
        builder.AddPass("c")
            .WithNext("d");
        builder.AddPass("d")
            .WithNext("b");

        Should.Throw<InvalidOperationException>(builder.Build);
    }

    [Fact]
    public void Build_Throws_WhenNextNotFound()
    {
        PassSequenceBuilder builder = new();
        builder.Root = "a";
        builder.AddPass("a")
            .WithNext("b");

        Should.Throw<InvalidOperationException>(builder.Build);
    }

    [Fact]
    public void Build_Throws_WhenRootNotFound()
    {
        PassSequenceBuilder builder = new();
        builder.Root = "a";

        Should.Throw<InvalidOperationException>(builder.Build);
    }

    [InlineData("<empty>")]
    [InlineData("<null>")]
    [InlineData("")]
    [InlineData(null)]
    [Theory]
    public void Build_Throws_WhenInvalidRoot(string? name)
    {
        PassSequenceBuilder builder = new();
        builder.Root = name!;
        builder.AddPass("a");

        Should.Throw<InvalidOperationException>(builder.Build);
    }
}
