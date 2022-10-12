using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Builders;

/// <summary>
/// A builder for a linked list of passes.
/// </summary>
public sealed class PassSequenceBuilder : IEnumerable<CompilerPassBuilder>
{
    private readonly Dictionary<string, CompilerPassBuilder> builders;
    private const string empty = "<empty>";

    /// <summary>
    /// The name of the root pass.
    /// </summary>
    public string Root { get; set; }



    /// <summary>
    /// Initializes a new <see cref="PassSequenceBuilder"/> instance.
    /// </summary>
    public PassSequenceBuilder()
    {
        builders = new();
        Root = "";
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
    public CompilerPassBuilder AddPass(string name)
    {
        if (IsReservedPassName(name))
        {
            throw new ArgumentException($"Name '{name}' is reserved", nameof(name));
        }

        if (builders.TryGetValue(name, out var b)) return b;

        CompilerPassBuilder builder = new(name);
        builders.Add(builder.Name, builder);

        return builder;
    }

    /// <summary>
    /// Adds a pass to the sequence.
    /// </summary>
    /// <param name="pass">The pass to add.</param>
    /// <returns>A new builder for the pass.</returns>
    public CompilerPassBuilder AddPass(CompilerPass pass) => AddPass(pass.Name)
        .WithDocumentation(pass.Documentation)
        .WithTransformations(pass.Transformations.Transformations)
        .WithPrevious(pass.Previous)
        .WithNext(pass.Next);

    /// <summary>
    /// Removes a pass from the sequence.
    /// </summary>
    /// <param name="name">The name of the pass to remove.</param>
    /// <returns>The current builder.</returns>
    public PassSequenceBuilder RemovePass(string name)
    {
        builders.Remove(name);
        return this;
    }

    /// <summary>
    /// Removes a pass from the sequence.
    /// </summary>
    /// <param name="pass">The pass to remove.</param>
    /// <returns>The current builder.</returns>
    public PassSequenceBuilder RemovePass(CompilerPassBuilder pass) =>
        RemovePass(pass.Name);

    /// <summary>
    /// Sets the root of the sequence.
    /// </summary>
    /// <param name="name">The name of the root.</param>
    /// <returns>A new builder for the pass.</returns>
    public CompilerPassBuilder SetRoot(string name)
    {
        var builder = AddPass(name);
        Root = builder.Name;
        return builder;
    }

    /// <summary>
    /// Sets the root of the sequence.
    /// </summary>
    /// <param name="pass">The pass to set the root to.</param>
    /// <returns>A new builder for the pass.</returns>
    /// <remarks>
    /// Ignores <see cref="CompilerPass.Previous"/>
    /// as the root builder does not have a previous pass.
    /// </remarks>
    public CompilerPassBuilder SetRoot(CompilerPass pass)
    {
        var builder = AddPass(pass)
            .WithPrevious(null);
        Root = builder.Name;
        return builder;
    }

    private static bool IsReservedPassName(string name) =>
        name is "<empty>" or "<null>";

    public IEnumerator<CompilerPassBuilder> GetEnumerator()
    {
        return builders.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Builds a <see cref="PassSequence"/> from the builder.
    /// </summary>
    public PassSequence Build()
    {
        if (!IsValidRoot(Root))
        {
            string rootName = Root ?? "<null>";
            throw new InvalidOperationException($"Root name '{rootName}' is not valid");
        }

        var passes = EnumerateBuilders()
            .Select(BuildPass)
            .Prepend(GetEmptyPass(Root));

        return PassSequence.Create(passes);
    }

    private static bool IsValidRoot(string root) =>
        root is not (null or "") && !IsReservedPassName(root);

    private IEnumerable<CompilerPassBuilder> EnumerateBuilders()
    {
        string targetName = Root;
        CompilerPassBuilder? previous = null;
        HashSet<string> built = new();

        while (true)
        {
            if (!builders.TryGetValue(targetName, out var current))
            {
                throw new InvalidOperationException($"Pass '{targetName}' does not exist");
            }

            if (built.Contains(targetName))
            {
                // An exception is going to be thrown anyway
                // so it doesn't matter that this isn't very efficient
                string[] refs = builders.Values
                    .Where(p => p.Next == targetName)
                    .Select(p => $"'{p.Name}'")
                    .ToArray();
                string refsMessage = refs.Length switch
                {
                    0 => "",
                    1 => $"{refs[0]}", // Wouldn't make sense if this happened, but for completeness' sake
                    2 => $"{refs[0]} and {refs[1]}",
                    _ => $"{string.Join(", ", refs[..^1])}, and {refs[^1]}"
                };
                throw new InvalidOperationException($"Circular reference in pass lineage: pass '{targetName}' is specified as the next pass for multiple passes ({refsMessage})");
            }

            if (current.Previous is not null && current.Previous != previous?.Name)
            {
                string currentName = current.Name;
                string currentPrevious = current.Previous;
                string previousName = previous?.Name ?? "<null>";
                throw new InvalidOperationException($"Inconsistent pass lineage: pass '{currentName}' specifies '{currentPrevious}' as its previous pass, expected '{previousName}' or null");
            }

            yield return current;

            if (current.Next is null) break;

            built.Add(targetName);
            targetName = current.Next;
            previous = current;
        }
    }

    private CompilerPass BuildPass(CompilerPassBuilder builder) => new(
        builder.Name,
        builder.Documentation,
        new PassTransformations(builder.Transformations.ToArray()),
        builder.Previous ?? empty,
        builder.Next
    );

    private static CompilerPass GetEmptyPass(string root) => new(
        empty,
        null,
        new PassTransformations(Array.Empty<ITransformationDescription>()),
        empty,
        root
    );

    /// <summary>
    /// Implicitly converts a <see cref="PassSequenceBuilder"/> into a <see cref="PassSequence"/>
    /// by calling <see cref="Build"/>.
    /// </summary>
    /// <param name="builder">The source builder.</param>
    public static implicit operator PassSequence(PassSequenceBuilder builder) =>
        builder.Build();
}
