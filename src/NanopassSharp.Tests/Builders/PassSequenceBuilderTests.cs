﻿using System;
using System.Xml.Linq;

namespace NanopassSharp.Builders.Tests;

public class PassSequenceBuilderTests
{
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
    [Theory]
    public void Build_Throws_WhenInvalidRoot(string name)
    {
        PassSequenceBuilder builder = new();
        builder.Root = name;
        builder.AddPass("a");

        Should.Throw<InvalidOperationException>(builder.Build);
    }
}
