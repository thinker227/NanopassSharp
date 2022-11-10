using NanopassSharp.LanguageHelpers;

namespace NanopassSharp.Languages.Yaml.Models;

internal sealed class AttributeModel
{
    public AccessibilityAttribute? Accessibility { get; init; }

    public string? Interface { get; init; }
}
