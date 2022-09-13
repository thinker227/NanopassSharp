using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for a <see cref="CompilerPass"/>.
/// </summary>
public sealed class CompilerPassBuilder
{
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Name" path="/summary"/>
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Documentation" path="/summary"/>
    /// </summary>
    public string? Documentation { get; set; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Transformations" path="/summary"/>
    /// </summary>
    public IList<ITransformationDescription> Transformations { get; set; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Previous" path="/summary"/>
    /// </summary>
    public string? Previous { get; set; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Next" path="/summary"/>
    /// </summary>
    public string? Next { get; set; }



    /// <summary>
    /// Initializes a new <see cref="CompilerPassBuilder"/> instance.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    /// <param name="previous"><inheritdoc cref="Previous" path="/summary"/></param>
    public CompilerPassBuilder(string name)
    {
        Name = name;
        Documentation = null;
        Transformations = new List<ITransformationDescription>();
        Previous = null;
        Next = null;
    }



    /// <summary>
    /// Creates a new <see cref="CompilerPassBuilder"/>
    /// from a <see cref="CompilerPass"/>.
    /// </summary>
    /// <param name="pass">The source pass.</param>
    public static CompilerPassBuilder FromPass(CompilerPass pass) =>
        new(pass.Name)
        {
            Documentation = pass.Documentation,
            Transformations = pass.Transformations.Transformations.ToList(),
            Previous = pass.Previous,
            Next = pass.Next
        };

    public CompilerPassBuilder WithDocumentation(string? documentation)
    {
        Documentation = documentation;
        return this;
    }

    public CompilerPassBuilder WithTransformations(IEnumerable<ITransformationDescription> transformations)
    {
        Transformations = transformations is IList<ITransformationDescription> list
            ? list
            : transformations.ToList();
        return this;
    }
    public CompilerPassBuilder AddTransformation(ITransformationDescription transformation)
    {
        Transformations.Add(transformation);
        return this;
    }

    public CompilerPassBuilder WithPrevious(string? previous)
    {
        Previous = previous;
        return this;
    }

    public CompilerPassBuilder WithNext(string? next)
    {
        Next = next;
        return this;
    }
}
