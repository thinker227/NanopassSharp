using System;
using System.IO;

namespace NanopassSharp.Cli;

internal readonly struct Options
{
    public string OutputLanguage { get; }

    public FileInfo PassFile { get; }

    public string InputLanguage { get; }

    public DirectoryInfo OutputLocation { get; }



    public Options(
        string outputLanguage,
        FileInfo passFile,
        string? inputLanguage,
        DirectoryInfo? outputLocation)
    {
        OutputLanguage = outputLanguage;
        PassFile = passFile;
        InputLanguage = inputLanguage ?? passFile.Extension[1..];
        OutputLocation = outputLocation ?? new(Environment.CurrentDirectory);
    }
}
