using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NanopassSharp.Cli.Input;

internal abstract record class Command
{
    public static Command Create(ParseResult parseResult)
    {
        List<InputError> errors = new(parseResult.Errors);
        InputHandler<Settings> handler = new(parseResult.Arguments, parseResult.Options, errors, new());

        if ((parseResult.Arguments.Length == 0 && parseResult.Options.Count == 0) ||
            handler.OptionIsSpecified("help", "h"))
        {
            return new Help();
        }

        if (handler.OptionIsSpecified("version", "v"))
        {
            return new Version();
        }

        handler.HandleArgument(0, "output-language", (args, outputLanguage, _) =>
            args.OutputLanguage = outputLanguage.Value);

        handler.HandleArgument(1, "pass-file", (args, passFile, errors) =>
        {
            if (!File.Exists(passFile.Value))
            {
                errors.Add(new(passFile.Index, $"File path '{passFile.Value}' does not exist"));
                return;
            }

            args.PassFile = new(passFile.Value);
        });

        handler.HandleOption("input-language", "i", (args, inputLanguage, _) =>
            args.InputLanguage = inputLanguage.Value);

        handler.HandleOption("output-location", "o", (args, outputLocation, errors) =>
        {
            if (!Directory.Exists(outputLocation.Value))
            {
                errors.Add(new(outputLocation.Index, $"Directory path '{outputLocation.Value}' does not exist"));
                return;
            }

            args.OutputLocation = new(outputLocation.Value);
        });

        handler.HandleBoolOption("print-options", null, (args, printOptions, _) =>
            args.PrintOptions = printOptions);

        handler.HandleUnhandledOptions((args, options, _) =>
            args.AdditionalOptions = options
                .Where(o => o.Signature.Kind == OptionKind.Long)
                .Where(o => o.Value is not null)
                .ToDictionary(o => o.Signature.Name, o => o.Value!.Value.Value));

        if (errors.Count > 0)
        {
            return new Error(errors);
        }

        return new Run(handler.Settings);
    }



    public sealed record class Run(Settings Arguments) : Command;

    public sealed record class Help : Command;

    public sealed record class Version : Command;

    public sealed record class Error(IReadOnlyCollection<InputError> Errors) : Command;
}
