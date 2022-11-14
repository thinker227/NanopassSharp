namespace NanopassSharp.Languages.Yaml.Models;

internal sealed class TransformationModel
{
    public required string Target { get; init; }

    public TransformationAddModel? Add { get; init; }

    public TransformationRemoveModel? Remove { get; init; }

    public TransformationEditModel? Edit { get; init; }
}
