namespace NanopassSharp;

/// <summary>
/// The behavior when creating a node from an already existing node.
/// </summary>
public enum CreateNodeBehavior
{
    /// <summary>
    /// The node will be created in place without creating any parents or children.
    /// </summary>
    CreateInPlace,
    /// <summary>
    /// The node will be created in place without creating any parents but including all children.
    /// </summary>
    CreateInPlaceWithChildren,
    /// <summary>
    /// The entire hierarchy from the root will, creating all parents and children.
    /// </summary>
    CreateFromRoot,
}
