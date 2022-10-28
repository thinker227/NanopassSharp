using System.Collections.Generic;
using System.IO;
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

    public override string GetTemplateString()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        using var resourceStream = assembly.GetManifestResourceStream(templateResourcePath);

        if (resourceStream is null)
        {
            throw new IOException($"Failed to read resource stream of resource '{templateResourcePath}'.");
        }

        StreamReader reader = new(resourceStream);
        string templateString = reader.ReadToEnd();

        return templateString;
    }
}
