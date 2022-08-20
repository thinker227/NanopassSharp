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
public sealed record class CompilerPass
(
    string Name,
    string? Documentation,
    PassTransformations Transformations,
    CompilerPass Previous
)
{
    /// <summary>
    /// The next pass immediately based on this pass.
    /// </summary>
    public CompilerPass? Next { get; set; }

    private Task<AstNodeHierarchy>? treeTask;
    /// <summary>
    /// Gets the transformed tree of this pass.
    /// </summary>
    public Task<AstNodeHierarchy> GetTreeAsync()
    {
        treeTask ??= GetTreeInternalAsync();
        return treeTask;
    }
    private async Task<AstNodeHierarchy> GetTreeInternalAsync()
    {
        var previousTree = await Previous.GetTreeAsync();
        return await PassTransformer.ApplyTransformationsAsync(previousTree, Transformations);
    }
}

/// <summary>
/// The transformations applied by a <see cref="CompilerPass"/>.
/// </summary>
/// <param name="Transformations">A list of transformations.</param>
public sealed record class PassTransformations
(
    IList<ITransformationDescription> Transformations
);

/// <summary>
/// A tree of nodes representing types which are the input and output of a pass.
/// </summary>
/// <param name="Roots">The root nodes.
/// May be multiple if the language does not support nested types.</param>
/// <param name="Nodes">The nodes of the tree.</param>
public sealed record class AstNodeHierarchy
(
    IList<AstNode> Roots,
    IDictionary<string, AstNode> Nodes
);

/// <summary>
/// A node representing a type in a compiler pass.
/// </summary>
/// <param name="Name">The name of the type.</param>
/// <param name="Documentation">The type's corresponding documentation.</param>
/// <param name="Parent">The parent type node.
/// <see langword="null"/> if the node is a root node.</param>
/// <param name="Children">The children of the node (typically nested types).</param>
/// <param name="Members">The members of the type.</param>
/// <param name="Attributes">The language-specific attributes of the type.</param>
public sealed record class AstNode
(
    string Name,
    string? Documentation,
    AstNode? Parent,
    IDictionary<string, AstNode> Children,
    IDictionary<string, AstNodeMember> Members,
    ISet<object> Attributes
);

/// <summary>
/// A member of a <see cref="AstNode"/>.
/// </summary>
/// <param name="Name">The name of the member.</param>
/// <param name="Documentation">The member's corresponding documentation.</param>
/// <param name="Type">The type of the member.
/// May be <see langword="null"/> in dynamically typed languages
/// or languages without explicitly typed members.</param>
/// <param name="Attributes">The language-specific attributes of the member.</param>
public sealed record class AstNodeMember
(
    string Name,
    string? Documentation,
    string? Type,
    ISet<object> Attributes
);
