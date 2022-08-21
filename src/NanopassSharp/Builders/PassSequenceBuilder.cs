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
    /// Builds a <see cref="PassSequence"/> from the builder.
    /// </summary>
    public PassSequence Build()
    {
        var dict = builders.ToDictionary(b => b.Name);
        LinkedList<CompilerPass> passes = new();

        var current = root;
        while (true)
        {
            passes.AddLast(current.Build());
            if (current.Next is null) break;
            if (!dict.TryGetValue(current.Next, out var next)) break;
            current = next;
        }
        return new PassSequence(passes);
    }
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
