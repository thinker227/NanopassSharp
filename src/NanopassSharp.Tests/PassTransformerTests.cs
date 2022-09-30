using System;
using NanopassSharp.Builders;

namespace NanopassSharp.Tests;

public class PassTransformerTests
{
    [Fact]
    public void TransformsEmptyTree()
    {
        AstNodeHierarchyBuilder builder = new();

        var a = builder.AddRoot("a");
        var b = a.AddChild("b");
        b.AddChild("c");
        b.AddChild("d");
        a.AddChild("e");

        var replacementTree = builder.Build();
        var originalTree = new AstNodeHierarchy(Array.Empty<AstNode>());

        var trans = new MockTransformation()
            .ApplyToTreeReturns(replacementTree);
        MockTransformationDescription desc = new() { Transformation = trans };
        PassTransformations transformations = new(new[] { desc });

        var transformedTree = PassTransformer.ApplyTransformations(originalTree, transformations);

        transformedTree.ShouldBe(replacementTree);
    }
}
