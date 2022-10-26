using System;
using System.Collections.Generic;
using System.IO;

namespace NanopassSharp.Cli;

internal sealed class Settings
{
    public string OutputLanguage { get; set; } = null!;

    public FileInfo PassFile { get; set; } = null!;

    public DirectoryInfo OutputLocation { get; set; } = new(Environment.CurrentDirectory);

    public string InputLanguage
    {
        get => inputLanguage
            ?? PassFile.Extension[1..];
        set => inputLanguage = value;
    }
    private string? inputLanguage;

    public bool PrintOptions { get; set; }

    public IReadOnlyDictionary<string, string> AdditionalOptions { get; set; } = null!;
}
