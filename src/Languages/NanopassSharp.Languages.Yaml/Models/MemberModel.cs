namespace NanopassSharp.Languages.Yaml.Models;

internal sealed class MemberModel
{
    public required string Name { get; init; }

    public string? Documentation { get; init; }

    public string? Type { get; init; }

    // TODO: Attributes
}
