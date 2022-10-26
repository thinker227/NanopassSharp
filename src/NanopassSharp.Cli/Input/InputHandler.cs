using System;
using System.Collections.Generic;

namespace NanopassSharp.Cli.Input;

internal sealed class InputHandler<TSettings>
{
    private readonly IndexString[] arguments;
    private readonly IReadOnlyDictionary<OptionSignature, Option> options;
    private readonly IList<InputError> errors;
    private readonly HashSet<Option> unhandledOptions;

    public TSettings Settings { get; }



    public InputHandler(IndexString[] arguments, IReadOnlyDictionary<OptionSignature, Option> options, IList<InputError> errors, TSettings settings)
    {
        this.arguments = arguments;
        this.options = options;
        this.errors = errors;
        Settings = settings;
        unhandledOptions = new(options.Values);
    }



    public void HandleArgument(int index, string argumentName, Action<TSettings, IndexString, IList<InputError>> handler)
    {
        if (index >= arguments.Length)
        {
            errors.Add(new(null, $"Missing required argument <{argumentName}>"));
            return;
        }

        var argument = arguments[index];
        handler(Settings, argument, errors);
    }

    public bool OptionIsSpecified(string longName, string? shortName)
    {
        OptionSignature longSignature = new(OptionKind.Long, longName);
        if (options.ContainsKey(longSignature)) return true;

        if (shortName is null) return false;

        OptionSignature shortSignature = new(OptionKind.Short, shortName);
        return options.ContainsKey(shortSignature);
    }

    public void HandleOption(string longName, string? shortName, Action<TSettings, IndexString, IList<InputError>> handler)
    {
        var o = GetOption(longName, shortName);
        if (o is not Option option) return;

        if (option.Value is null)
        {
            string optionName = shortName is not null
                ? $"--{longName}|-{shortName}"
                : $"--{longName}";
            errors.Add(new(null, $"Missing value for option [{optionName}]"));
            return;
        }

        handler(Settings, option.Value.Value, errors);
        unhandledOptions.Remove(option);
    }

    public void HandleBoolOption(string longName, string? shortName, Action<TSettings, bool, IList<InputError>> handler)
    {
        var o = GetOption(longName, shortName);
        if (o is not Option option) return;

        bool value;

        if (option.Value is null)
        {
            value = true;
        }
        else
        {
            if (bool.TryParse(option.Value.Value.Value, out bool result))
            {
                value = result;
            }
            else
            {
                errors.Add(new(null, $"Could not parse '{option.Value.Value.Value}' to a boolean value"));
                return;
            }
        }

        handler(Settings, value, errors);
        unhandledOptions.Remove(option);
    }

    public Option? GetOption(string longName, string? shortName = null)
    {
        OptionSignature longSignature = new(OptionKind.Long, longName);
        if (options.TryGetValue(longSignature, out var longOption)) return longOption;

        if (shortName is null) return null;

        OptionSignature shortSignature = new(OptionKind.Short, shortName);
        if (options.TryGetValue(shortSignature, out var shortOption)) return shortOption;

        return null;
    }

    public void HandleUnhandledOptions(Action<TSettings, IReadOnlyCollection<Option>, IList<InputError>> handler)
    {
        handler(Settings, unhandledOptions, errors);
        unhandledOptions.Clear();
    }
}
