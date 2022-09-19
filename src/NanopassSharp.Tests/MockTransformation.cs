using System;

namespace NanopassSharp.Tests;

internal sealed class MockTransformationDescription : ITransformationDescription
{
    public ITransformationPattern? Pattern { get; set; }

    public ITransformation Transformation { get; set; } = new MockTransformation();
}

internal sealed class MockTransformationPattern : ITransformationPattern
{
    public bool IsRecursive { get; set; } = false;
    private Func<AstNodeHierarchy, bool> isMatchTreeFunc =
        _ => true;
    private Func<AstNodeHierarchy, AstNode, bool> isMatchNodeFunc =
        (_, _) => true;
    private Func<AstNodeHierarchy, AstNode, AstNodeMember, bool> isMatchMemberFunc =
        (_, _, _) => true;



    /// <summary>
    /// Sets the function for <see cref="IsMatch(AstNodeHierarchy)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchTreeReturns(Func<AstNodeHierarchy, bool> func)
    {
        isMatchTreeFunc = func;
        return this;
    }

    /// <summary>
    /// Sets the return value for <see cref="IsMatch(AstNodeHierarchy)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchTreeReturns(bool value) =>
        IsMatchTreeReturns(_ => value);

    /// <summary>
    /// Sets the function for <see cref="IsMatch(AstNodeHierarchy, AstNode)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchNodeReturns(Func<AstNodeHierarchy, AstNode, bool> func)
    {
        isMatchNodeFunc = func;
        return this;
    }

    /// <summary>
    /// Sets the return value for <see cref="IsMatch(AstNodeHierarchy, AstNode)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchNodeReturns(bool value) =>
        IsMatchNodeReturns((_, _) => value);

    /// <summary>
    /// Sets the function for <see cref="IsMatch(AstNodeHierarchy, AstNode, AstNodeMember)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchMemberReturns(Func<AstNodeHierarchy, AstNode, AstNodeMember, bool> func)
    {
        isMatchMemberFunc = func;
        return this;
    }

    /// <summary>
    /// Sets the return value for <see cref="IsMatch(AstNodeHierarchy, AstNode, AstNodeMember)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchMemberReturns(bool value) =>
        IsMatchMemberReturns((_, _, _) => value);

    public bool IsMatch(AstNodeHierarchy tree) => isMatchTreeFunc(tree);
    public bool IsMatch(AstNodeHierarchy tree, AstNode node) => isMatchNodeFunc(tree, node);
    public bool IsMatch(AstNodeHierarchy tree, AstNode node, AstNodeMember member) => isMatchMemberFunc(tree, node, member);
}

internal sealed class MockTransformation : ITransformation
{
    private Func<AstNodeHierarchy, AstNode, AstNodeMember, AstNodeMember> applyToMemberFunc =
        (_, _, member) => member;
    private Func<AstNodeHierarchy, AstNode, AstNode> applyToNodeFunc =
        (_, node) => node;
    private Func<AstNodeHierarchy, AstNodeHierarchy> applyToTreeFunc =
        tree => tree;



    /// <summary>
    /// Sets the function for <see cref="ApplyToMember(AstNodeHierarchy, AstNode, AstNodeMember)"/>.
    /// </summary>
    public MockTransformation ApplyToMemberReturns(Func<AstNodeHierarchy, AstNode, AstNodeMember, AstNodeMember> func)
    {
        applyToMemberFunc = func;
        return this;
    }

    /// <summary>
    /// Sets the return value for <see cref="ApplyToMember(AstNodeHierarchy, AstNode, AstNodeMember)"/>.
    /// </summary>
    public MockTransformation ApplyToMemberReturns(AstNodeMember value) =>
        ApplyToMemberReturns((_, _, _) => value);

    /// <summary>
    /// Sets the function for <see cref="ApplyToNode(AstNodeHierarchy, AstNode)"/>.
    /// </summary>
    public MockTransformation ApplyToNodeReturns(Func<AstNodeHierarchy, AstNode, AstNode> func)
    {
        applyToNodeFunc = func;
        return this;
    }

    /// <summary>
    /// Sets the return value for <see cref="ApplyToNode(AstNodeHierarchy, AstNode)"/>.
    /// </summary>
    public MockTransformation ApplyToNodeReturns(AstNode value) =>
        ApplyToNodeReturns((_, _) => value);

    /// <summary>
    /// Sets the function for <see cref="ApplyToTree(AstNodeHierarchy)"/>.
    /// </summary>
    public MockTransformation ApplyToTreeReturns(Func<AstNodeHierarchy, AstNodeHierarchy> func)
    {
        applyToTreeFunc = func;
        return this;
    }

    /// <summary>
    /// Sets the return value for <see cref="ApplyToTree(AstNodeHierarchy)"/>.
    /// </summary>
    public MockTransformation ApplyToTreeReturns(AstNodeHierarchy value) =>
        ApplyToTreeReturns(_ => value);

    public AstNodeMember ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        applyToMemberFunc(tree, node, member);

    public AstNode ApplyToNode(AstNodeHierarchy tree, AstNode node) =>
        applyToNodeFunc(tree, node);

    public AstNodeHierarchy ApplyToTree(AstNodeHierarchy tree) =>
        applyToTreeFunc(tree);
}
