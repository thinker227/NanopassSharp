using System;
using NanopassSharp.Builders;

namespace NanopassSharp.Tests;

public class PassTransformerTests
{
    [Fact]
    public void ReturnsOriginalTreeIfNoTransformations()
    {
        AstNodeHierarchyBuilder builder = new();
        builder.AddRoot("a");

        var tree = builder.Build();
        var transformations = Array.Empty<ITransformationDescription>();

        var transformedTree = PassTransformer.ApplyTransformations(tree, transformations);

        transformedTree.ShouldBeSameAs(tree);
    }

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
        var transformations = new[] { desc };

        var transformedTree = PassTransformer.ApplyTransformations(originalTree, transformations);

        transformedTree.ShouldBe(replacementTree);
    }

    [Fact]
    public void TransformsMembers()
    {
        AstNodeHierarchyBuilder originalBuilder = new();
        {
            var foo = originalBuilder.AddRoot("foo");
            foo.AddMember("a")
                .WithDocumentation("an a")
                .WithType(null)
                .AddAttribute(true);
        }
        var originalTree = originalBuilder.Build();

        var modifyMemberTransform = new MockTransformation()
            .ApplyToMemberReturns((_, _, member) => AstNodeMemberBuilder
                .FromMember(member)
                .WithDocumentation("a glorious a")
                .WithType("type a")
                .AddAttribute("attribute")
                .AddAttribute(10)
                .Build());
        var modifyMemberPattern = new MockTransformationPattern()
            .IsMatchTreeReturns(false)
            .IsMatchNodeReturns(false)
            .IsMatchMemberReturns((_, _, member) => member.Name == "a");
        var modifyMemberDescription = new MockTransformationDescription()
        {
            Transformation = modifyMemberTransform,
            Pattern = modifyMemberPattern
        };

        var addMemberTransform = new MockTransformation()
            .ApplyToNodeReturns((_, node) =>
            {
                var nodeBuilder = new AstNodeHierarchyBuilder()
                    .CreateNode(node);
                var member = new AstNodeMemberBuilder("b")
                    .WithDocumentation("a fine b")
                    .WithType("type b")
                    .AddAttribute(false)
                    .Build();
                nodeBuilder.AddMember(member);

                return nodeBuilder.Build();
            });
        var addMemberPattern = new MockTransformationPattern()
            .IsMatchTreeReturns(false)
            .IsMatchNodeReturns(true)
            .IsMatchMemberReturns(false);
        var addMemberDescription = new MockTransformationDescription()
        {
            Transformation = addMemberTransform,
            Pattern = addMemberPattern
        };

        var transformations = new[] { modifyMemberDescription, addMemberDescription };

        AstNodeHierarchyBuilder expectedBuilder = new();
        {
            var foo = expectedBuilder.AddRoot("foo");
            foo.AddMember("a")
                .WithDocumentation("a glorious a")
                .WithType("type a")
                .AddAttribute(true)
                .AddAttribute("attribute")
                .AddAttribute(10);
            foo.AddMember("b")
                .WithDocumentation("a fine b")
                .WithType("type b")
                .AddAttribute(false);
        }
        var expected = expectedBuilder.Build();

        var transformedTree = PassTransformer.ApplyTransformations(originalTree, transformations);

        transformedTree.ShouldBe(expected);
    }
}
