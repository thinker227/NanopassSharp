namespace NanopassSharp.Descriptions;

/// <summary>
/// A simple implementation of <see cref="ITransformationDescription"/>.
/// </summary>
/// <param name="Pattern"><inheritdoc cref="ITransformationDescription.Pattern" path="/summary"/></param>
/// <param name="Transformation"><inheritdoc cref="ITransformationDescription.Transformation" path="/summary"/></param>
public sealed record class SimpleDescription(
    ITransformationPattern? Pattern,
    ITransformation Transformation
) : ITransformationDescription;
