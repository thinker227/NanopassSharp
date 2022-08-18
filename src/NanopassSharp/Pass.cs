using System.Collections.Generic;
using System.Threading.Tasks;

namespace NanopassSharp;

/// <summary>
/// A single compiler pass.
/// </summary>
/// <param name="Name">The name of the pass.</param>
/// <param name="Documentation">The pass' corresponding documentation.</param>
/// <param name="Transformations">The transformations the pass applies to its nodes.</param>
/// <param name="Previous">The previous pass which this pass is based on.</param>
public sealed record class Pass
(
    string Name,
    string? Documentation,
    PassTransformations Transformations,
    Pass Previous
)
{

    /// <summary>
    /// The next pass immediately based on this pass.
    /// </summary>
    public Pass? Next { get; set; }

    private Task<TypeNodeTree>? treeTask;
    /// <summary>
    /// Gets the transformed tree of this pass.
    /// </summary>
    public Task<TypeNodeTree> GetTreeAsync()
    {
        treeTask ??= GetTreeInternalAsync();
        return treeTask;
    }
    private async Task<TypeNodeTree> GetTreeInternalAsync()
    {
        var previousTree = await Previous.GetTreeAsync();
        return await PassTransformer.ApplyTransformationsAsync(previousTree, Transformations);
    }

}

/// <summary>
/// The transformations applied by a <see cref="Pass"/>.
/// </summary>
/// <param name="Transformations">A list of transformations.</param>
public sealed record class PassTransformations
(
    IList<ITransformation> Transformations
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
