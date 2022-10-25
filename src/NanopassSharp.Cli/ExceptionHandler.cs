using System;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NanopassSharp.Cli;

internal static class ExceptionHandler
{
    public static void Handle(Exception exception)
    {
        var console = AnsiConsole.Console;

        if (exception is CommandRuntimeException)
        {
            console.MarkupLine($"[deeppink3]{exception.Message}[/]");
            return;
        }

        const ExceptionFormats format
            = ExceptionFormats.ShortenTypes
            ;

        ExceptionStyle style = new()
        {
            Message = new(Color.DeepPink3),
            ParameterType = new(Color.SpringGreen2),
            Method = new(Color.LightGoldenrod1),
            Path = new(Color.MediumPurple2),
            LineNumber = new(Color.IndianRed1_1),
        };

        ExceptionSettings settings = new()
        {
            Format = format,
            Style = style
        };

        console.MarkupLine("[deeppink3]An internal exception has occured.[/]");
        console.WriteException(exception, settings);
    }
}
