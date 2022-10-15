using System;
using System.Collections.Generic;
using System.Linq;
using NanopassSharp.Builders;
using ITransformations = System.Collections.Generic.IReadOnlyCollection<NanopassSharp.ITransformationDescription>;

namespace NanopassSharp;

/// <summary>
/// Transforms AST node hierarchies.
/// </summary>
public static class PassTransformer
{
    /// <summary>
    /// Applies transformations to a <see cref="AstNodeHierarchy"/>.
    /// </summary>
    /// <param name="tree">The tree to transform.</param>
    /// <param name="transformations">The transformations to apply.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static AstNodeHierarchy ApplyTransformations(AstNodeHierarchy tree, ITransformations transformations)
    {
        if (transformations.Count == 0)
        {
            return tree;
        }

        tree = ApplyTransformationsToTree(tree, transformations);

        var builder = AstNodeHierarchyBuilder.FromHierarchy(tree);

        TraverseRoots(builder, tree, transformations);

        return builder.Build();
    }

    /// <summary>
    /// Traverses all the roots of a tree.
    /// </summary>
    /// <param name="builder">The builder which is being transformed.</param>
    /// <param name="tree">The tree which is being transformed.</param>
    /// <param name="transformations">The transformations to apply.</param>
    private static void TraverseRoots(AstNodeHierarchyBuilder builder, AstNodeHierarchy tree, ITransformations transformations)
    {
        foreach (var root in tree.Roots)
        {
            NodePath path = new(root.Name);
            TraverseNode(builder, path, root, tree, transformations);
        }
    }

    /// <summary>
    /// Traverses a node and applies any applicable transformation on the node and all children.
    /// </summary>
    /// <param name="builder">The builder which is being transformed.</param>
    /// <param name="path">The path to the currently traversed node. Should be the exact path to <paramref name="node"/>.</param>
    /// <param name="node">The node which is currently being traversed. Should have the exact same path as <paramref name="path"/>.</param>
    /// <param name="tree">The tree which is being transformed.</param>
    /// <param name="transformations">The transformation to try applying.</param>
    private static void TraverseNode(AstNodeHierarchyBuilder builder, NodePath path, AstNode node, AstNodeHierarchy tree, ITransformations transformations)
    {
        var transformedNode = ApplyTransformationsToNode(tree, node, transformations);

        if (transformedNode is null)
        {
            builder.RemoveNode(path);
            return;
        }

        // Path *should* never be invalid, the node should always exist,
        // otherwise there's an error in the algorithm somewhere.
        var nodeBuilder = builder.GetNodeFromPath(path)!;
        UpdateNode(nodeBuilder, transformedNode);

        UpdateChildren(builder, path, node.Children, transformedNode.Children);

        // Transform members
        foreach (var member in transformedNode.Members.Values)
        {
            VisitMember(builder, path, member, transformedNode, tree, transformations);
        }

        // Transform children
        foreach (var (childName, child) in transformedNode.Children)
        {
            var childPath = path.CreateLeafPath(childName);
            TraverseNode(builder, childPath, child, tree, transformations);
        }
    }

    /// <summary>
    /// Visits a member and applies any applicable transformations on the member.
    /// </summary>
    /// <param name="builder">The builder which is being transformed.</param>
    /// <param name="nodePath">The path to the parent node of the member. Should be the exact path to <paramref name="node"/>.</param>
    /// <param name="member">The member which is currently being visited.</param>
    /// <param name="node">The parent node of the member. Should have the exact same path as <paramref name="path"/>.</param>
    /// <param name="tree">The tree which is being transformed.</param>
    /// <param name="transformations"></param>
    private static void VisitMember(AstNodeHierarchyBuilder builder, NodePath nodePath, AstNodeMember member, AstNode node, AstNodeHierarchy tree, ITransformations transformations)
    {
        // Again *should* never be null, otherwise the algorithm is wrong
        var nodeBuilder = builder.GetNodeFromPath(nodePath)!;

        var transformedMember = ApplyTransformationsToMember(tree, node, member, transformations);

        if (transformedMember is null)
        {
            nodeBuilder.RemoveMember(member.Name);
            return;
        }

        // This is currently the only way to get a member
        // with a specific name from a node builder
        var memberBuilder = nodeBuilder.Members.First(c => c.Name == member.Name);
        UpdateMember(memberBuilder, transformedMember);
    }

    /// <summary>
    /// Sequentially applies a list of transformations onto a tree and returns the resulting tree.
    /// </summary>
    /// <param name="tree">The tree which should be transformed.</param>
    /// <param name="transformations">The transformations to apply.</param>
    /// <returns>A new tree which is the result of applying <paramref name="transformations"/> onto <paramref name="tree"/>.</returns>
    private static AstNodeHierarchy ApplyTransformationsToTree(AstNodeHierarchy tree, ITransformations transformations)
    {
        var transformed = tree;

        foreach (var trans in transformations)
        {
            if (trans.Pattern?.IsMatch(transformed) ?? true)
            {
                tree = trans.Transformation.ApplyToTree(transformed);
            }
        }

        return tree;
    }

    /// <summary>
    /// Sequentially applies a list of transformations onto a node and returns the resulting node.
    /// </summary>
    /// <param name="tree">The tree the node is a part of.</param>
    /// <param name="node">The node which should be transformed.</param>
    /// <param name="transformations">The transformations to apply.</param>
    /// <returns>A new node which is the result of applying <paramref name="transformations"/> onto <paramref name="node"/>.</returns>
    private static AstNode? ApplyTransformationsToNode(AstNodeHierarchy tree, AstNode node, ITransformations transformations)
    {
        var transformed = node;

        foreach (var trans in transformations)
        {
            if (transformed is null)
            {
                return transformed;
            }

            if (trans.Pattern?.IsMatch(tree, node) ?? true)
            {
                transformed = trans.Transformation.ApplyToNode(tree, node);
            }
        }

        return transformed;
    }

    /// <summary>
    /// Sequentially applies a list of transformations onto a member and returns the resulting member.
    /// </summary>
    /// <param name="tree">The tree the member is a part of.</param>
    /// <param name="node">The parent node of the member..</param>
    /// <param name="member">The member which should be transformed.</param>
    /// <param name="transformations">The transformations to apply.</param>
    /// <returns>A new member which is the result of applying <paramref name="transformations"/> onto <paramref name="member"/>.</returns>
    private static AstNodeMember? ApplyTransformationsToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member, ITransformations transformations)
    {
        var transformed = member;

        foreach (var trans in transformations)
        {
            if (transformed is null)
            {
                return transformed;
            }

            if (trans.Pattern?.IsMatch(tree, node, member) ?? true)
            {
                transformed = trans.Transformation.ApplyToMember(tree, node, member);
            }
        }

        return transformed;
    }

    /// <summary>
    /// Updates a <see cref="AstNodeBuilder"/> with the contents of an <see cref="AstNode"/>.
    /// </summary>
    /// <param name="builder">The builder to update.</param>
    /// <param name="transformedNode">The node to update the builder with.</param>
    private static void UpdateNode(AstNodeBuilder builder, AstNode transformedNode) => builder
        .WithAttributes(new HashSet<object>(transformedNode.Attributes))
        .WithDocumentation(transformedNode.Documentation)
        .WithChildren(transformedNode.Children.Keys);

    /// <summary>
    /// Updates a <see cref="AstNodeMemberBuilder"/> with the contents of an <see cref="AstNodeMember"/>.
    /// </summary>
    /// <param name="builder">The builder to update.</param>
    /// <param name="transformedMember">The member to update the builder with.</param>
    private static void UpdateMember(AstNodeMemberBuilder builder, AstNodeMember transformedMember) => builder
        .WithName(transformedMember.Name)
        .WithDocumentation(transformedMember.Documentation)
        .WithType(transformedMember.Type)
        .WithAttributes(new HashSet<object>(transformedMember.Attributes));

    /// <summary>
    /// Updates the children of a path.
    /// </summary>
    /// <param name="builder">The builder which is being transformed.</param>
    /// <param name="path">The path to the node which children are being updated.</param>
    /// <param name="originalChildren">The original children of the node.</param>
    /// <param name="modifiedChildren">The children of the modified node.</param>
    private static void UpdateChildren(AstNodeHierarchyBuilder builder, NodePath path, IReadOnlyDictionary<string, AstNode> originalChildren, IReadOnlyDictionary<string, AstNode> modifiedChildren)
    {
        foreach (string originalChild in originalChildren.Keys)
        {
            var childPath = path.CreateLeafPath(originalChild);

            // The child has been removed
            if (!modifiedChildren.ContainsKey(originalChild))
            {
                builder.RemoveNode(childPath);
                // The children of the child don't really need to be removed
                // as they will be either discarded or overwritten later on.
                continue;
            }

            // The child remains, nothing has changed
            // There's no need to do anything here because the children
            // of the child will be handled once the method returns.
        }

        foreach (string modifiedChild in modifiedChildren.Keys)
        {
            var childPath = path.CreateLeafPath(modifiedChild);

            // The child has been added
            if (!originalChildren.ContainsKey(modifiedChild))
            {
                builder.CreateNode(childPath);
                continue;
            }

            // Again, the child remains, nothing has changed
        }
    }
}
