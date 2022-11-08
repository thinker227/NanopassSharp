using System.Collections.Generic;

namespace NanopassSharp.Languages.Yaml.Models;

internal sealed class PassModel
{
    public required string Name { get; init; }

    public string? Documentation { get; init; }

    public string? Next { get; init; }

    public string? Previous { get; init; }

    public required List<TransformationModel> Transformations { get; init; }
}
