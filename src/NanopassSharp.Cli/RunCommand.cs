using System;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace NanopassSharp.Cli;

internal sealed class RunCommand : AsyncCommand<RunSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, RunSettings settings)
    {
        Console.WriteLine($"Input language: {settings.InputLanguage}");
        Console.WriteLine($"Output language: {settings.OutputLanguage}");
        Console.WriteLine($"Pass file path: {settings.PassFile.FullName}");
        Console.WriteLine($"Output path: {settings.OutputLocation.FullName}");

        return Task.FromResult(0);
    }
}
