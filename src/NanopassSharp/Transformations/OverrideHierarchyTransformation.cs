namespace NanopassSharp.Transformations;

/// <summary>
/// A transformation which overrides a hierarchy with a new hierarchy.
/// </summary>
public sealed class OverrideHierarchyTransformation : BaseTransformation
{
    /// <summary>
    /// The hierarchy used to override.
    /// </summary>
    public AstNodeHierarchy Hierarchy { get; }

    /// <summary>
    /// Initializes a new <see cref="OverrideHierarchyTransformation"/> instance.
    /// </summary>
    /// <param name="hierarchy"><inheritdoc cref="Hierarchy" path="/summary"/></param>
    public OverrideHierarchyTransformation(AstNodeHierarchy hierarchy)
    {
        Hierarchy = hierarchy;
    }

    public override AstNodeHierarchy ApplyToTree(AstNodeHierarchy tree) =>
        Hierarchy;
}
