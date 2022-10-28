using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scriban;

namespace NanopassSharp.LanguageHelpers;

/// <summary>
/// A base implementation of <see cref="IOutputLanguage"/> using Scriban
/// to generate text from a template.
/// </summary>
/// <remarks>
/// See <see href="https://github.com/scriban/scriban"/> for more information about Scriban.
/// </remarks>
public abstract class ScribanOutputLanguage : IOutputLanguage
{
    public abstract IEnumerable<string> Aliases { get; }



    /// <summary>
    /// Gets the string template to use.
    /// </summary>
    /// <remarks>
    /// The context object provided when rendering the template is the same object
    /// returned by <see cref="GetModel(AstNodeHierarchy)"/>,
    /// which by default is the emitted <see cref="AstNodeHierarchy"/>.
    /// </remarks>
    public abstract string GetTemplateString();

    /// <summary>
    /// Gets a <see cref="Template"/> from a template string.
    /// </summary>
    /// <param name="templateString">The template string to get the <see cref="Template"/> from.</param>
    public virtual Template GetTemplate(string templateString) =>
        Template.Parse(templateString);

    /// <summary>
    /// Gets the model object to use when rendering.
    /// </summary>
    /// <param name="tree">The node hierarchy.</param>
    /// <remarks>
    /// By default returns <paramref name="tree"/>.
    /// </remarks>
    public virtual object GetModel(AstNodeHierarchy tree) =>
        tree;

    public async Task<string> EmitAsync(EmitContext context, CancellationToken cancellationToken)
    {
        string templateString = GetTemplateString();
        var template = GetTemplate(templateString);
        object model = GetModel(context.Hierarchy);

        cancellationToken.ThrowIfCancellationRequested();

        return await template.RenderAsync(model);
    }
}
