using NanopassSharp.Descriptions;

namespace NanopassSharp.Transformations;

public static class TransformationExtensions
{
    /// <summary>
    /// Creates an <see cref="ITransformationDescription"/> from a pattern and a transformation.
    /// </summary>
    /// <param name="transformation">The transformation.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>A new transformation description.</returns>
    public static ITransformationDescription WithPattern(this ITransformation transformation, ITransformationPattern? pattern) =>
        new SimpleDescription(pattern, transformation);
}
