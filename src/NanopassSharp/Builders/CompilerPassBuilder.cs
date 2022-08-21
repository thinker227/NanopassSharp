using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for a <see cref="CompilerPass"/>.
/// </summary>
public sealed class CompilerPassBuilder : ISequentialCompilerPassBuilder
{
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Name" path="/summary"/>
    /// </summary>
    public string Name { get; set; }
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
    public string Previous { get; set; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Next" path="/summary"/>
    /// </summary>
    public string? Next { get; set; }



    /// <summary>
    /// Initializes a new <see cref="CompilerPassBuilder"/> instance.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    /// <param name="previous"><inheritdoc cref="Previous" path="/summary"/></param>
    public CompilerPassBuilder(string name, string previous)
    {
        Name = name;
        Documentation = null;
        Transformations = new List<ITransformationDescription>();
        Previous = previous;
        Next = null;
    }
    /// <summary>
    /// Initializes a new <see cref="CompilerPassBuilder"/> instance.
    /// </summary>
    /// <param name="pass">The <see cref="CompilerPass"/> to create the builder from.</param>
    public CompilerPassBuilder(CompilerPass pass)
    {
        Name = pass.Name;
        Documentation = pass.Documentation;
        Transformations = pass.Transformations.Transformations.ToList();
        Previous = pass.Previous;
        Next = pass.Next;
    }



    /// <summary>
    /// Sets the name of the pass.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public CompilerPassBuilder WithName(string name)
    {
        Name = name;
        return this;
    }
    /// <summary>
    /// Sets the documentation of the pass.
    /// </summary>
    /// <param name="documentation"><inheritdoc cref="Documentation" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public CompilerPassBuilder WithDocumentation(string? documentation)
    {
        Documentation = documentation;
        return this;
    }
    ISequentialCompilerPassBuilder ISequentialCompilerPassBuilder.WithDocumentation(string? documentation) =>
        WithDocumentation(documentation);
    /// <summary>
    /// Adds a transformation to the pass.
    /// </summary>
    /// <param name="transformation">The transformation to add.</param>
    /// <returns>The current builder.</returns>
    public CompilerPassBuilder AddTransformation(ITransformationDescription transformation)
    {
        Transformations.Add(transformation);
        return this;
    }
    ISequentialCompilerPassBuilder ISequentialCompilerPassBuilder.AddTransformation(ITransformationDescription transformation) =>
        AddTransformation(transformation);
    /// <summary>
    /// Sets the previous pass of the pass.
    /// </summary>
    /// <param name="previous"><inheritdoc cref="Previous" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public CompilerPassBuilder WithPrevious(string previous)
    {
        Previous = previous;
        return this;
    }
    /// <summary>
    /// Sets the next pass of the pass.
    /// </summary>
    /// <param name="next"><inheritdoc cref="Next" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    public CompilerPassBuilder WithNext(string? next)
    {
        Next = next;
        return this;
    }

    /// <summary>
    /// Builds a <see cref="CompilerPass"/> from the builder.
    /// </summary>
    public CompilerPass Build() =>
        new(Name, Documentation, new(Transformations.ToList()), Previous, Next);
    /// <summary>
    /// Implicitly converts a <see cref="CompilerPassBuilder"/> to a <see cref="CompilerPass"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator CompilerPass(CompilerPassBuilder builder) =>
        builder.Build();
}

// This is just for the sake of pass builders created using a PassSequenceBuilder
// not being able to modify the name, previous, or next properties.
/// <summary>
/// A sequential builder for a <see cref="CompilerPass"/>.
/// </summary>
public interface ISequentialCompilerPassBuilder
{
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Name" path="/summary"/>
    /// </summary>
    string Name { get; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Documentation" path="/summary"/>
    /// </summary>
    string? Documentation { get; set; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Previous" path="/summary"/>
    /// </summary>
    string Previous { get; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Next" path="/summary"/>
    /// </summary>
    string? Next { get; }
    /// <summary>
    /// <inheritdoc cref="CompilerPass.Transformations" path="/summary"/>
    /// </summary>
    IList<ITransformationDescription> Transformations { get; set; }

    /// <summary>
    /// Sets the documentation of the pass.
    /// </summary>
    /// <param name="documentation"><inheritdoc cref="Documentation" path="/summary"/></param>
    /// <returns>The current builder.</returns>
    ISequentialCompilerPassBuilder WithDocumentation(string? documentation);
    /// <summary>
    /// Adds a transformation to the pass.
    /// </summary>
    /// <param name="transformation">The transformation to add.</param>
    /// <returns>The current builder.</returns>
    ISequentialCompilerPassBuilder AddTransformation(ITransformationDescription transformation);
    /// <summary>
    /// Builds a <see cref="CompilerPass"/> from the builder.
    /// </summary>
    CompilerPass Build();
}
