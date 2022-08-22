using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NanopassSharp;

public static class PassExtensions
{
    public static IEnumerable<AstNode> GetNodes(this AstNodeHierarchy tree) =>
        tree.Roots.SelectMany(r => r.GetDecendantNodesAndSelf());
    public static IEnumerable<AstNode> GetDecendantNodes(this AstNode node) =>
        node.Children.Values.SelectMany(n => n.GetDecendantNodes());
    public static IEnumerable<AstNode> GetDecendantNodesAndSelf(this AstNode node) =>
        node.GetDecendantNodes().Prepend(node);
    public static AstNode GetRoot(this AstNode node) =>
        node.Parent?.GetRoot() ?? node;
    public static IEnumerable<string> GetPathFromRoot(this AstNode node) =>
        node.Parent is null
            ? Enumerable.Empty<string>()
            : node.Parent.GetPathFromRoot().Append(node.Name);
    public static IEnumerable<string> GetPathWithRoot(this AstNode node) =>
        node.Parent is null
            ? new[] { node.Name }
            : node.Parent.GetPathWithRoot().Append(node.Name);

    public static AstNodeHierarchy ReplaceNode(this AstNodeHierarchy tree, AstNode oldNode, AstNode newNode)
    {
        var root = oldNode.GetRoot();
        if (!tree.Roots.Contains(root)) throw new ArgumentException("Node does not exist in the tree", nameof(oldNode));

        var parent = oldNode.Parent;
        if (parent is null) return tree with
        {
            Roots = tree.Roots.ToImmutableArray().Replace(oldNode, newNode)
        };

        string name = oldNode.Name;
        var node = newNode with
        {
            Name = name
        };
        var newParent = parent with
        {
            Children = parent.Children.ToImmutableDictionary().SetItem(name, node)
        };
        return tree.ReplaceNode(parent, newParent);
    }
    public static AstNodeHierarchy RemoveNode(this AstNodeHierarchy tree, AstNode node)
    {
        var root = node.GetRoot();
        if (!tree.Roots.Contains(root)) throw new ArgumentException("Node does not exist in the tree", nameof(node));

        var parent = node.Parent;
        if (parent is null) return tree with
        {
            Roots = tree.Roots.ToImmutableArray().Remove(node)
        };

        string name = node.Name;
        var newParent = parent with
        {
            Children = parent.Children.ToImmutableDictionary().Remove(name)
        };
        return tree.ReplaceNode(parent, newParent);
    }
    
    public static AstNode ReplaceMember(this AstNode node, AstNodeMember oldMember, AstNodeMember newMember)
    {
        string name = oldMember.Name;
        if (!node.Members.ContainsKey(name)) throw new ArgumentException("Member does not exist in the node", nameof(oldMember));

        var member = newMember with
        {
            Name = name
        };
        return node with
        {
            Members = node.Members.ToImmutableDictionary().SetItem(name, member)
        };
    }
    public static AstNode RemoveMember(this AstNode node, AstNodeMember member)
    {
        string name = member.Name;
        if (!node.Members.ContainsKey(name)) throw new ArgumentException("Member does not exist in the node", nameof(member));

        return node with
        {
            Members = node.Members.ToImmutableDictionary().Remove(name)
        };
    }
}
