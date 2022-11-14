using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using NanopassSharp.Languages.Yaml.Models;
using NanopassSharp.Builders;
using NanopassSharp.Patterns;
using NanopassSharp.Transformations;
using NanopassSharp.Descriptions;
using NanopassSharp.LanguageHelpers;

namespace NanopassSharp.Languages.Yaml;

public sealed class YamlInputLanguage : IInputLanguage
{
    public Task<PassSequence> GeneratePassSequenceAsync(InputContext context, CancellationToken cancellationToken)
    {
        var deserializer = GetDeserializer();
        var passes = deserializer.Deserialize<List<PassModel>>(context.Text);

        var sequenceBuilder = new PassSequenceBuilder()
        {
            Root = passes[0].Name
        };

        //var rootPass = passes[0].Transformations.Select(transform => transform.Add)

        foreach (var passModel in passes)
        {
            AddPass(sequenceBuilder, passModel);
        }

        return Task.FromResult(sequenceBuilder.Build());
    }

    private static IDeserializer GetDeserializer() => new DeserializerBuilder()
        .WithNamingConvention(HyphenatedNamingConvention.Instance)
        .Build();

    private static void AddPass(PassSequenceBuilder sequence, PassModel model)
    {
        var pass = sequence.AddPass(model.Name)
            .WithDocumentation(model.Documentation)
            .WithNext(model.Next)
            .WithPrevious(model.Previous);

        foreach (var transform in model.Transformations)
        {
            AddTransforms(pass, transform);
        }
    }

    private static void AddTransforms(CompilerPassBuilder pass, TransformationModel model)
    {
        var target = NodePath.ParseUnsafe(model.Target);

        var removeTransforms = model.Remove is not null
            ? GetTransformsFromRemoveModel(model.Remove, target)
            : Enumerable.Empty<ITransformationDescription>();

        var editTransforms = model.Edit is not null
            ? GetTransformsFromEditModel(model.Edit, target)
            : Enumerable.Empty<ITransformationDescription>();

        var addTransforms = model.Add is not null
            ? GetTransformsFromAddModel(model.Add, target)
            : Enumerable.Empty<ITransformationDescription>();

        var transforms = removeTransforms
            .Concat(editTransforms)
            .Concat(addTransforms);

        foreach (var trans in transforms)
        {
            pass.AddTransformation(trans);
        }
    }

    private static IEnumerable<ITransformationDescription> GetTransformsFromRemoveModel(TransformationRemoveModel remove, NodePath target)
    {
        List<NodePath> targets = new();

        if (remove.Child is string child)
        {
            targets.Add(target.CreateLeafPath(child));
        }

        if (remove.Member is string member)
        {
            targets.Add(target.CreateLeafPath(member));
        }

        object? attribute = remove.Attribute is AttributeModel attributeModel
            ? ToAttribute(attributeModel)
            : null;
        var attributeTransform = attribute is not null
            ? new LambdaBuilderTransformation()
            {
                MemberTransformer = (_, _, builder) =>
                    builder.Attributes.Remove(attribute),

                NodeTransformer = (_, builder) =>
                    builder.Attributes.Remove(attribute)
            }
            : null;

        var transform = Transform.Remove;
        var transforms = targets.Select(path => new SimpleDescription(Pattern.Path(path), transform));
        return attributeTransform is not null
            ? transforms.Append(new SimpleDescription(Pattern.Path(target), attributeTransform))
            : transforms;
    }

    private static IEnumerable<ITransformationDescription> GetTransformsFromEditModel(TransformationEditModel edit, NodePath target)
    {
        List<ITransformation> transforms = new();

        if (edit.Name is string editName)
        {
            var transform = new LambdaBuilderTransformation()
            {
                MemberTransformer = (_, _, builder) =>
                    builder.Name = editName
            };

            transforms.Add(transform);
        }

        // Note:
        // If the type or documentation is not specified (null), the value will not be changed.
        // Otherwise, an empty or whitespace value will be interpreted as the value being removed.

        if (edit.Type is string editType)
        {
            string? type = string.IsNullOrWhiteSpace(editType)
                ? null
                : editType;

            var transform = new LambdaBuilderTransformation()
            {
                MemberTransformer = (_, _, builder) =>
                    builder.Type = type
            };

            transforms.Add(transform);
        }

        if (edit.Documentation is string editDocumentation)
        {
            string? doc = string.IsNullOrWhiteSpace(editDocumentation)
                ? null
                : editDocumentation;

            var transform = new LambdaBuilderTransformation()
            {
                MemberTransformer = (_, _, builder) =>
                    builder.Documentation = doc,

                NodeTransformer = (_, builder) =>
                    builder.Documentation = doc
            };

            transforms.Add(transform);
        }

        var pattern = Pattern.Path(target);
        return transforms.Select(trans => new SimpleDescription(pattern, trans));
    }

    private static IEnumerable<ITransformationDescription> GetTransformsFromAddModel(TransformationAddModel add, NodePath target)
    {
        List<ITransformation> transforms = new();

        if (add.Node is NodeModel nodeModel)
        {
            var node = ToNode(nodeModel);

            var transform = new LambdaBuilderTransformation()
            {
                NodeTransformer = (_, builder) =>
                    builder.AddChild(node)
            };

            transforms.Add(transform);
        }

        if (add.Member is MemberModel memberModel)
        {
            var member = ToMember(memberModel);

            var transform = new LambdaBuilderTransformation()
            {
                NodeTransformer = (_, builder) =>
                    builder.AddMember(member)
            };

            transforms.Add(transform);
        }

        if (add.Attribute is AttributeModel attributeModel && ToAttribute(attributeModel) is object attribute)
        {
            var transform = new LambdaBuilderTransformation()
            {
                MemberTransformer = (_, _, builder) =>
                    builder.AddAttribute(attribute),

                NodeTransformer = (_, builder) =>
                    builder.AddAttribute(attribute)
            };

            transforms.Add(transform);
        }

        var pattern = Pattern.Path(target);
        return transforms.Select(trans => new SimpleDescription(pattern, trans));
    }

    private static AstNode ToNode(NodeModel model)
    {
        AstNodeHierarchyBuilder hierarchyBuilder = new();
        var root = hierarchyBuilder.CreateNode(model.Name);

        UpdateNodeToModel(root, model);

        return root.Build();
    }

    private static void UpdateNodeToModel(AstNodeBuilder builder, NodeModel model)
    {
        builder.Documentation = model.Documentation;

        if (model.Attributes is not null)
        {
            builder.Attributes = ToAttributes(model.Attributes);
        }

        foreach (var member in model.Members ?? Enumerable.Empty<MemberModel>())
        {
            builder.AddMember(ToMember(member));
        }

        foreach (var childModel in model?.Children ?? Enumerable.Empty<NodeModel>())
        {
            var child = builder.AddChild(childModel.Name);
            UpdateNodeToModel(child, childModel);
        }
    }

    private static AstNodeMember ToMember(MemberModel model)
    {
        var builder = new AstNodeMemberBuilder(model.Name)
            .WithDocumentation(model.Documentation)
            .WithType(model.Type);

        if (model.Attributes is not null)
        {
            builder.Attributes = ToAttributes(model.Attributes);
        }

        return builder.Build();
    }

    private static object? ToAttribute(AttributeModel model)
    {
        if (model.Accessibility is not null) return model.Accessibility;

        if (model.Interface is not null) return new InterfaceAttribute(model.Interface);

        return null;
    }

    private static ISet<object> ToAttributes(IEnumerable<AttributeModel> models) => models
        .Select(ToAttribute)
        .Where(a => a is not null)
        .ToHashSet()!;
}
