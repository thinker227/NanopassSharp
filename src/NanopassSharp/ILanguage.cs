using System.Threading.Tasks;

namespace NanopassSharp;

/// <summary>
/// Defines a language.
/// </summary>
public interface ILanguage : IAstNodeHierarchyLocator
{
    /// <summary>
    /// The name of the language.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Emits a hierarchy as source code for the current language.
    /// </summary>
    /// <param name="hierarchy">The hierarchy to emit.</param>
    Task EmitHierarchyAsync(AstNodeHierarchy hierarchy);
}

/// <summary>
/// A locator for the source hierarchy of an execution.
/// </summary>
public interface IAstNodeHierarchyLocator
{
    /// <summary>
    /// Gets the source hierarchy.
    /// </summary>
    AstNode GetRootNode();
}

/// <summary>
/// A provider for a <see cref="ILanguage"/>.
/// </summary>
public interface ILanguageProvider
{
    /// <summary>
    /// The pattern which indicates that the language should be used.
    /// </summary>
    ILanguagePattern Pattern { get; }

    /// <summary>
    /// Creates a new <see cref="ILanguage"/> asynchronously.
    /// </summary>
    /// <param name="context">The context for the initialization of the current execution.</param>
    /// <returns>A new <see cref="ILanguage"/>.</returns>
    Task<ILanguage> CreateLanguageAsync(InitializationContext context);
}

/// <summary>
/// A pattern for whether a language should be used.
/// </summary>
public interface ILanguagePattern
{
    /// <summary>
    /// Returns whether the current initialization context matches the requirement for the language.
    /// </summary>
    /// <param name="context">The context for the initialization of the current execution.</param>
    /// <returns></returns>
    Task<bool> IsMatchAsync(InitializationContext context);
}
