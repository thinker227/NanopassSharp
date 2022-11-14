using System.Collections.Generic;

namespace NanopassSharp.Languages.Yaml.Models;

internal sealed class RootElementModel
{
    public required NodeModel Root { get; init; }

    public required List<PassModel> Passes { get; init; }
}
