using System.IO;
using System.CommandLine;
using System.CommandLine.Builder;

namespace NanopassSharp.Cli;

internal static class CommandLineConfigurer
{
    public static RootCommand GetRootCommand()
    {
        RootCommand rootCommand = new()
        {
            Name = "nanopass-sharp",
            Description = "The Nanopass# framework"
        };

        Argument<string> outputLanguageArg = new(
            name: "output-language",
            description: "The name of the output language");
        rootCommand.AddArgument(outputLanguageArg);

        Argument<FileInfo> passFileArg = new(
            name: "pass-file",
            description: "The path to the file containing the pass definitions");
        passFileArg.AddValidator(result =>
        {
            var file = result.GetValueOrDefault<FileInfo>();

            if (!file.Exists)
            {
                result.ErrorMessage = $"File '{file.FullName}' does not exist";
                return;
            }
        });
        rootCommand.AddArgument(passFileArg);

        Option<string?> inputLanguageOption = new(
            name: "--input-language",
            description: "The name of the input language. Defaults to the extension of the pass file");
        inputLanguageOption.AddAlias("-i");
        rootCommand.AddOption(inputLanguageOption);

        Option<DirectoryInfo?> outputLocationOption = new(
            name: "--output-location",
            description: "The path to the output location. Defaults to the current directory");
        outputLocationOption.AddAlias("-o");
        outputLocationOption.AddValidator(result =>
        {
            var directory = result.GetValueOrDefault<DirectoryInfo>();

            if (!directory!.Exists)
            {
                result.ErrorMessage = $"Directory '{directory.FullName}' does not exist";
                return;
            }
        });
        rootCommand.AddOption(outputLocationOption);
        
        rootCommand.SetHandler(async (outputLanguage, passFile, inputLanguage, outputLocation) =>
        {
            Options options = new(
                outputLanguage,
                passFile,
                inputLanguage,
                outputLocation);

            Runner runner = new(options);
            await runner.RunAsync();
        },
            outputLanguageArg,
            passFileArg,
            inputLanguageOption,
            outputLocationOption);

        return rootCommand;
    }

    public static CommandLineBuilder GetRootBuilder()
    {
        var command = GetRootCommand();

        CommandLineBuilder builder = new(command);

        builder.UseDefaults();

        return builder;
    }
}
