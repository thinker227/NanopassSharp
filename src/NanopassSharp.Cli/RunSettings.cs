using System;
using System.ComponentModel;
using System.IO;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NanopassSharp.Cli;

internal sealed class RunSettings : CommandSettings
{
    [CommandArgument(0, "<output-language>")]
    [Description("The name of the output language (ex. 'c#', 'csharp', 'js', 'javascript')")]
    public string OutputLanguage { get; init; } = null!;

    [CommandArgument(1, "<pass-file>")]
    [Description("The path to the file containing pass definitions")]
    public string PassFilePath { get; init; } = null!;
    public FileInfo PassFile => new(PassFilePath);

    [CommandOption("-o|--output")]
    [Description("The path to the output directory. Defaults to the current directory")]
    public string OutputLocationPath { get; init; } = Environment.CurrentDirectory;
    public DirectoryInfo OutputLocation => new(OutputLocationPath);

    [CommandOption("-i|--input-language")]
    [Description("The name of the input language. Defaults to the extension of the pass file")]
    public string InputLanguage
    {
        get => inputLanguage
            ?? PassFile.Extension[1..];
        init => inputLanguage = value;
    }
    private string? inputLanguage;

    [CommandOption("--print-options", IsHidden = true)]
    public bool PrintOptions { get; init; }



    public override ValidationResult Validate()
    {
        if (!File.Exists(PassFilePath))
        {
            return ValidationResult.Error($"File '{PassFilePath}' does not exist.");
        }

        if (!Directory.Exists(OutputLocationPath))
        {
            return ValidationResult.Error($"Directory '{OutputLocationPath}' does not exist.");
        }

        return ValidationResult.Success();
    }
}
