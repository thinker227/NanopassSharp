using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp;

/// <summary>
/// A linked sequence of passes.
/// </summary>
public sealed class PassSequence : IReadOnlyList<CompilerPass>
{
    private readonly Dictionary<CompilerPass, AstNodeHierarchy> trees;
    private readonly LinkedList<CompilerPass> passes;

    public CompilerPass this[int index] => passes.ElementAt(index);
    public int Count => passes.Count;
    /// <summary>
    /// The passes as a dictionary, with the name of each pass being a key in the dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, CompilerPass> Passes { get; }
    /// <summary>
    /// Gets a pass by its name.
    /// </summary>
    /// <param name="name">The name of the pass.</param>
    public CompilerPass this[string name] => Passes[name];



    /// <summary>
    /// Initializes a new <see cref="PassSequence"/> instance.
    /// </summary>
    /// <param name="passes">The passes as a linked list.</param>
    public PassSequence(LinkedList<CompilerPass> passes)
    {
        trees = new();
        this.passes = passes;
        Passes = passes.ToDictionary(p => p.Name);
    }



    /// <summary>
    /// Gets the tree of a pass by the pass' name.
    /// </summary>
    /// <param name="passName">The name of the pass to get the tree of.</param>
    /// <returns>The <see cref="AstNodeHierarchy"/> of the pass with the name <paramref name="passName"/>.</returns>
    public AstNodeHierarchy GetTree(string passName) =>
        GetTree(Passes[passName]);
    /// <summary>
    /// Gets the tree of a specified pass.
    /// </summary>
    /// <param name="pass">The pass to get the tree of.</param>
    /// <returns>The <see cref="AstNodeHierarchy"/> of <paramref name="pass"/>.</returns>
    public AstNodeHierarchy GetTree(CompilerPass pass)
    {
        if (trees.TryGetValue(pass, out var memoized)) return memoized;

        AstNodeHierarchy tree;
        var previousPass = Passes[pass.Previous];
        if (pass.Name == previousPass.Name)
        {
            tree = AstNodeHierarchy.Empty;
        }
        else
        {
            var previousTree = GetTree(previousPass);
            tree = PassTransformer.ApplyTransformations(previousTree, pass.Transformations);
        }

        trees.Add(pass, tree);
        return tree;
    }

    public IEnumerator<CompilerPass> GetEnumerator() =>
        passes.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}
