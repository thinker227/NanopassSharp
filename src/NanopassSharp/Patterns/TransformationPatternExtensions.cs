using NanopassSharp.Descriptions;

namespace NanopassSharp.Patterns;

public static class TransformationPatternExtensions
{
    /// <summary>
    /// Returns a new <see cref="ITransformationPattern"/> which applies a binary and operation to two patterns
    /// (<c><paramref name="left"/> &amp;&amp; <paramref name="right"/></c>).
    /// </summary>
    /// <param name="left">The left pattern.</param>
    /// <param name="right">The right pattern.</param>
    public static ITransformationPattern And(this ITransformationPattern left, ITransformationPattern right) =>
        new BinaryPattern(left, right, (a, b) => a && b);

    /// <summary>
    /// Returns a new <see cref="ITransformationPattern"/> which applies a binary or operation to two patterns
    /// (<c><paramref name="left"/> || <paramref name="right"/></c>).
    /// </summary>
    /// <param name="left">The left pattern.</param>
    /// <param name="right">The right pattern.</param>
    public static ITransformationPattern Or(this ITransformationPattern left, ITransformationPattern right) =>
        new BinaryPattern(left, right, (a, b) => a || b);

    /// <summary>
    /// Returns a new <see cref="ITransformationPattern"/> which applies a binary exclusive or operation to two patterns
    /// (<c><paramref name="left"/> ^ <paramref name="right"/></c>).
    /// </summary>
    /// <param name="left">The left pattern.</param>
    /// <param name="right">The right pattern.</param>
    public static ITransformationPattern Xor(this ITransformationPattern left, ITransformationPattern right) =>
        new BinaryPattern(left, right, (a, b) => a ^ b);

    /// <summary>
    /// Returns a new <see cref="ITransformationPattern"/> which applies a unary not operation to a pattern.
    /// (<c>!<paramref name="operand"/></c>).
    /// </summary>
    /// <param name="operand">The operand.</param>
    public static ITransformationPattern Not(this ITransformationPattern operand) =>
        new UnaryPattern(operand, x => !x);

    /// <summary>
    /// Creates an <see cref="ITransformationDescription"/> from a pattern and a transformation.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="transformation">The transformation.</param>
    /// <returns>A new transformation description.</returns>
    public static ITransformationDescription WithTransformation(this ITransformationPattern? pattern, ITransformation transformation) =>
        new SimpleDescription(pattern, transformation);
}
