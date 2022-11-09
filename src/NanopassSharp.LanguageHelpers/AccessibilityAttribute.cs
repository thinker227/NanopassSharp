namespace NanopassSharp.LanguageHelpers;

/// <summary>
/// An attribute which represents the accessibility of a node or member.
/// </summary>
public enum AccessibilityAttribute
{
    /// <summary>
    /// The node or member is publicly accessible.
    /// Equivalent to <see langword="public"/>.
    /// </summary>
    Public,
    /// <summary>
    /// The node or member is only accessible within the containing node.
    /// Equivalent to <see langword="private"/>.
    /// </summary>
    Containing,
    /// <summary>
    /// The node or member is only accessible within the current project/assembly.
    /// Equivalent to <see langword="internal"/>.
    /// </summary>
    Project,
    /// <summary>
    /// The node or member is only accessible within the file.
    /// Equivalent to <see langword="file"/>.
    /// </summary>
    File
}
