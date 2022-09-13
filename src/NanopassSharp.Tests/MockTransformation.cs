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
    private Func<AstNode, bool> isMatchNodeFunc =
        (_) => true;
    private Func<AstNode, AstNodeMember, bool> isMatchMemberFunc =
        (_, _) => true;



    /// <summary>
    /// Sets the function for <see cref="IsMatch(AstNode)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchNodeReturns(Func<AstNode, bool> func)
    {
        isMatchNodeFunc = func;
        return this;
    }
    /// <summary>
    /// Sets the return value for <see cref="IsMatch(AstNode)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchNodeReturns(bool value) =>
        IsMatchNodeReturns((_) => value);
    /// <summary>
    /// Sets the function for <see cref="IsMatch(AstNode, AstNodeMember)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchMemberReturns(Func<AstNode, AstNodeMember, bool> func)
    {
        isMatchMemberFunc = func;
        return this;
    }
    /// <summary>
    /// Sets the return value for <see cref="IsMatch(AstNode, AstNodeMember)"/>.
    /// </summary>
    public MockTransformationPattern IsMatchMemberReturns(bool value) =>
        IsMatchMemberReturns((_, _) => value);

    public bool IsMatch(AstNode node) => isMatchNodeFunc(node);
    public bool IsMatch(AstNode node, AstNodeMember member) => isMatchMemberFunc(node, member);
}

internal sealed class MockTransformation : ITransformation
{
    private Func<AstNodeHierarchy, AstNode, AstNodeMember, AstNodeMember> applyToMemberFunc =
        (_, _, member) => member;
    private Func<AstNodeHierarchy, AstNode, AstNode> applyToNodeFunc =
        (_, node) => node;



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

    public AstNodeMember ApplyToMember(AstNodeHierarchy tree, AstNode node, AstNodeMember member) =>
        applyToMemberFunc(tree, node, member);
    public AstNode ApplyToNode(AstNodeHierarchy tree, AstNode node) =>
        applyToNodeFunc(tree, node);
}
