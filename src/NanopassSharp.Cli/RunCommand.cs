using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;

namespace NanopassSharp.Cli;

internal sealed class RunCommand
{
    private Settings settings = null!;



    public RunCommand(Settings settings)
    {
        this.settings = settings;
    }



    public Task<int> ExecuteAsync()
    {
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

        if (settings.AdditionalOptions.Count > 0)
        {
            AnsiConsole.MarkupLine("\n[gray]Additional options:[/]");
            foreach (var (name, value) in settings.AdditionalOptions)
            {
                AnsiConsole.MarkupLine($"[white]--{name} {value}[/]");
            }
        }
        
    }
}
