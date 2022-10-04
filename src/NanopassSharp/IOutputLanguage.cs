using System.Collections.Generic;
using System.Threading.Tasks;

namespace NanopassSharp;

/// <summary>
/// Defines a language.
/// </summary>
public interface IOutputLanguage
{
    /// <summary>
    /// The aliases which identify the language.
    /// </summary>
    IEnumerable<string> Aliases { get; }

    /// <summary>
    /// Emits an <see cref="AstNodeHierarchy"/>.
    /// </summary>
    /// <param name="context">The context of the current execution.</param>
    /// <returns>The emitted code/text.</returns>
    Task<string> EmitAsync(EmitContext context);
}
