using System.Threading.Tasks;

namespace NanopassSharp;

/// <summary>
/// Defines a language.
/// </summary>
public interface ILanguage
{
    /// <summary>
    /// The name of the language.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Emits an <see cref="AstNodeHierarchy"/> to the current context.
    /// </summary>
    /// <param name="hierarchy">The hierarchy to emit.</param>
    /// <param name="context">The context of the current execution.</param>
    Task EmitAsync(AstNodeHierarchy hierarchy, ExecutionContext context);
}

/// <summary>
/// A provider for a single language.
/// </summary>
public interface ILanguageProvider
{
    /// <summary>
    /// The pattern which determines whether the language should be used.
    /// </summary>
    ILanguagePattern Pattern { get; }

    /// <summary>
    /// Creates a language.
    /// </summary>
    /// <param name="context">The current execution context.</param>
    /// <returns>A new <see cref="ILanguage"/>.</returns>
    Task<ILanguage> CreateLanguageAsync(ExecutionContext context);
}

/// <summary>
/// A pattern which determines whether a language should be used.
/// </summary>
public interface ILanguagePattern
{
    /// <summary>
    /// Returns whether to use the language based on the current context.
    /// </summary>
    /// <param name="context">The current execution context.</param>
    Task<bool> IsMatchAsync(ExecutionContext context);
}
