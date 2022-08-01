using System.Collections.Generic;

namespace NanopassSharp;

/// <summary>
/// A single compiler pass.
/// </summary>
/// <param name="Name">The name of the pass.</param>
/// <param name="Documentation">The pass' corresponding documentation.</param>
/// <param name="Transformations">The transformations the pass applies to its nodes.</param>
/// <param name="Tree">The original node tree of the pass.</param>
public sealed record class Pass
(
    string Name,
    string? Documentation,
    Pass Previous,
    IList<ITransformation> Transformations,
    // Should this be immutable?
    // Should the tree be the tree resulting from applying the pass' transformations?
    // Should it be created immediately by the language-specific parser
    // or be based on the previous pass?
    TypeNodeTree Tree
);

/// <summary>
/// A tree of nodes representing types which are the input and output of a pass.
/// </summary>
/// <param name="Root">The root node.</param>
/// <param name="Nodes">The nodes of the tree.</param>
public sealed record class TypeNodeTree
(
    TypeNode Root,
    IDictionary<string, TypeNode> Nodes
);

/// <summary>
/// A node representing a type in a compiler pass.
/// </summary>
/// <param name="Name">The name of the type.</param>
/// <param name="Documentation">The type's corresponding documentation.</param>
/// <param name="Parent">The parent type node.
/// <see langword="null"/> if the node is the root node.</param>
/// <param name="Children">The children of the node (typically nested types).</param>
/// <param name="Members">The members of the type.</param>
/// <param name="Attributes">The language-specific attributes of the type.</param>
public sealed record class TypeNode
(
    string Name,
    string? Documentation,
    TypeNode? Parent,
    IDictionary<string, TypeNode> Children,
    IDictionary<string, Member> Members,
    ISet<object> Attributes
);

/// <summary>
/// A member of a type.
/// </summary>
/// <param name="Name">The name of the member.</param>
/// <param name="Documentation">The member's corresponding documentation.</param>
/// <param name="Type">The type of the member.
/// May be <see langword="null"/> in dynamically typed languages
/// or languages without explicitly typed members.</param>
/// <param name="Attributes">The language-specific attributes of the member.</param>
public sealed record class Member
(
    string Name,
    string? Documentation,
    string? Type,
    ISet<object> Attributes
);

class C
{
    static void M()
    {
        Member member = new(null!, null, null, null!);
    }
}
