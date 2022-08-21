using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NanopassSharp.Cli;

internal sealed class RunCommand : AsyncCommand<RunCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        public Settings()
        {
            projectFile = new(() => FindProjectFile(ProjectPath));
            passesFile = new(() =>
            {
                FileInfo file = new(PassesPath);
                return file.Exists ? file : null;
            });
        }

        [CommandArgument(0, "[project-path]")]
        [Description("The path to the .csproj file or the directory containing it")]
        public string ProjectPath { get; init; } = "";
        [CommandArgument(1, "[passes-path]")]
        [Description("The path to the YAML file containing the passes")]
        public string PassesPath { get; init; } = "";
        [CommandOption("--debug")]
        [Description("Whether to run in debug mode")]
        public bool Debug { get; init; }

        private readonly Lazy<FileInfo?> projectFile;
        public FileInfo? ProjectFile => projectFile.Value;

        private readonly Lazy<FileInfo?> passesFile;
        public FileInfo? PassesFile => passesFile.Value;

        public override ValidationResult Validate()
        {
            if (ProjectFile is null)
            {
                return ValidationResult.Error($"Project '{ProjectPath}' could not be found");
            }

            if (PassesFile is null)
            {
                return ValidationResult.Error($"Passes '{PassesPath}' could not be found");
            }

            return ValidationResult.Success();
        }
    }



    public override Task<int> ExecuteAsync(CommandContext context, Settings settings) =>
        Task.FromResult(0);

    private static FileInfo? FindProjectFile(string path)
    {
        if (File.Exists(path) && Path.HasExtension(".csproj"))
        {
            return new(path);
        }

        if (Directory.Exists(path))
        {
            DirectoryInfo dir = new(path);
            var files = dir.GetFiles("*.csproj");
            return files.Length == 1 ? files[0] : null;
        }

        return null;
    }
}
