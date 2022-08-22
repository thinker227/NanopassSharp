using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for a linked list of passes.
/// </summary>
public sealed class PassSequenceBuilder : IReadOnlyList<ISequentialCompilerPassBuilder>
{
    private readonly CompilerPassBuilder root;
    private readonly List<CompilerPassBuilder> builders;
    private const string rootName = "<rootPass>";

    public ISequentialCompilerPassBuilder this[int index] => builders[index];
    public int Count => builders.Count;



    /// <summary>
    /// Initializes a new <see cref="PassSequenceBuilder"/> instance.
    /// </summary>
    public PassSequenceBuilder()
    {
        root = new(rootName, rootName);
        builders = new() { root };
    }



    /// <summary>
    /// Creates a new <see cref="PassSequenceBuilder"/>
    /// from a <see cref="PassSequence"/>.
    /// </summary>
    /// <param name="sequence">The source sequence.</param>
    public static PassSequenceBuilder FromSequence(PassSequence sequence)
    {
        PassSequenceBuilder builder = new();
        foreach (var pass in sequence)
        {
            builder.AddPass(pass);
        }
        return builder;
    }

    /// <summary>
    /// Adds a pass to the sequence.
    /// </summary>
    /// <param name="name">The name of the pass.</param>
    /// <returns>A new builder for the pass.</returns>
    public ISequentialCompilerPassBuilder AddPass(string name, string? documentation = null)
    {
        var last = builders[^1];
        var builder = new CompilerPassBuilder(name, last.Name).WithDocumentation(documentation);
        builders.Add(builder);
        last.Next = builder.Name;
        return builder;
    }
    /// <summary>
    /// Adds a pass to the sequence.
    /// </summary>
    /// <param name="pass">The pass to add.</param>
    /// <returns>A new builder for the pass.</returns>
    public ISequentialCompilerPassBuilder AddPass(CompilerPass pass)
    {
        var last = builders[^1];
        var builder = CompilerPassBuilder.FromPass(pass);
        builders.Add(builder);
        builder.Previous = last.Name;
        last.Next = builder.Name;
        return builder;
    }

    /// <summary>
    /// Builds a <see cref="PassSequence"/> from the builder.
    /// </summary>
    public PassSequence Build() =>
        PassSequence.Create(builders.Select(b => b.Build()), root.Build());
    /// <summary>
    /// Implicitly converts a <see cref="PassSequenceBuilder"/> into a <see cref="PassSequence"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator PassSequence(PassSequenceBuilder builder) =>
        builder.Build();

    public IEnumerator<ISequentialCompilerPassBuilder> GetEnumerator() =>
        builders.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}
