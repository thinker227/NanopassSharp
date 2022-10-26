using System.Collections.Generic;
using NanopassSharp.Cli.Input;
using Spectre.Console;

internal class ErrorReporter
{
    private readonly IReadOnlyCollection<InputError> errors;



    public ErrorReporter(IReadOnlyCollection<InputError> errors)
    {
        this.errors = errors;
    }



    internal int Report()
    {
        foreach (var error in errors)
        {
            AnsiConsole.MarkupLine($"[red]{error.Message}[/]");
        }

        return 1;
    }
}
