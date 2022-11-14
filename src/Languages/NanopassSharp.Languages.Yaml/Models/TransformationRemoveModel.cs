﻿namespace NanopassSharp.Languages.Yaml.Models;

internal sealed class TransformationRemoveModel
{
    public string? Child { get; init; }

    public string? Member { get; init; }

    public AttributeModel? Attribute { get; init; }
}