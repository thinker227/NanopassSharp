using System.Collections.Generic;

namespace NanopassSharp.Languages.Yaml.Models;

internal sealed class NodeModel
{
    public required string Name { get; init; }

    public string? Documentation { get; init; }

    public List<NodeModel>? Children { get; init; }

    public List<MemberModel>? Members { get; init; }

    public List<AttributeModel>? Attributes { get; init; }
}
