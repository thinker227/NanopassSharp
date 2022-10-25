using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NanopassSharp.Cli;

internal sealed class RunCommand : AsyncCommand<RunSettings>
{
    private RunSettings settings = null!;

    public override Task<int> ExecuteAsync(CommandContext context, RunSettings settings)
    {
        this.settings = settings;

        if (settings.PrintOptions)
        {
            PrintOptions();
        }

        return Task.FromResult(0);
    }

    private void PrintOptions()
    {
        AnsiConsole.MarkupLine($"[aqua]Options specified:[/]");
        AnsiConsole.MarkupLine($"[gray]Input language:[/] [white]{settings.InputLanguage}[/]");
        AnsiConsole.MarkupLine($"[gray]Output language:[/] [white]{settings.OutputLanguage}[/]");
        AnsiConsole.MarkupLine($"[gray]Pass file path:[/] [white]{settings.PassFile.FullName}[/]");
        AnsiConsole.MarkupLine($"[gray]Output path:[/] [white]{settings.OutputLocation.FullName}[/]");
        AnsiConsole.MarkupLine($"[gray]Print options:[/] [white]{settings.PrintOptions}[/]");
    }
}
