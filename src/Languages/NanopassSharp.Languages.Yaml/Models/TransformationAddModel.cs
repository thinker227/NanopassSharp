namespace NanopassSharp.Languages.Yaml.Models;

internal sealed class TransformationAddModel
{
    public MemberModel? Member { get; init; }

    public NodeModel? Node { get; init; }

    // TODO: Allowing adding an attribute
}
