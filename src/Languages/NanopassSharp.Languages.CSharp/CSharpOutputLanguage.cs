using System.Collections.Generic;
using NanopassSharp.LanguageHelpers;

namespace NanopassSharp.Languages.CSharp;

public sealed class CSharpOutputLanguage : ScribanOutputLanguage
{
    public override IEnumerable<string> Aliases { get; } = new[]
    {
        "csharp",
        "cs",
        "c#",
    };

    private const string templateResourcePath = "NanopassSharp.Languages.CSharp.template.sbncs";

    public override string GetTemplateString() =>
        ManifestResourceHelper.ReadResourceAsStringOrThrow(templateResourcePath);
}
