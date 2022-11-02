namespace NanopassSharp.Descriptions;

/// <summary>
/// A pair of an <see cref="ITransformationPattern"/> and an <see cref="ITransformation"/>.
/// </summary>
/// <param name="Pattern"><inheritdoc cref="ITransformationDescription.Pattern" path="/summary"/></param>
/// <param name="Transformation"><inheritdoc cref="ITransformationDescription.Transformation" path="/summary"/></param>
public sealed record class SimpleDescription(
    ITransformationPattern? Pattern,
    ITransformation Transformation
) : ITransformationDescription;
