using System.Collections.Generic;

namespace NanopassSharp.Cli.Input;

internal sealed class Parser
{
    private int index;
    private readonly Token[] tokens;
    private readonly List<IndexString> arguments;
    private readonly Dictionary<OptionSignature, Option> options;
    private readonly List<Error> errors;



    private Parser(Token[] tokens)
    {
        index = 0;
        this.tokens = tokens;
        arguments = new();
        options = new();
        errors = new();
    }



    public static ParseResult Parse(string[] args)
    {
        var tokens = GetTokens(args);
        Parser parser = new(tokens);

        parser.Parse();

        var arguments = parser.arguments.ToArray();
        var options = parser.options;
        var errors = parser.errors;

        return new(arguments, options, errors);
    }

    private static Token[] GetTokens(string[] args)
    {
        var tokens = new Token[args.Length];

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            TokenKind kind;
            if (arg.StartsWith("--"))
            {
                kind = TokenKind.LongOption;
            }
            else if (arg.StartsWith('-'))
            {
                kind = TokenKind.ShortOption;
            }
            else
            {
                kind = TokenKind.Value;
            }

            tokens[i] = new(i, arg, kind);
        }

        return tokens;
    }

    private void Parse()
    {
        bool parsingOptions = false;

        while (index < tokens.Length)
        {
            var t = Peek();
            if (t is not Token token) break;

            if (token.Kind != TokenKind.Value)
            {
                parsingOptions = true;
            }

            if (parsingOptions)
            {
                var o = Option();
                if (o is Option option)
                {
                    options.Add(option.Signature, option);
                }
            }
            else
            {
                Next();
                arguments.Add(new(token.Index, token.Value));
            }
        }
    }

    private Option? Option()
    {
        var t = Next();

        if (t is not Token token)
        {
            return null;
        }

        if (token.Kind == TokenKind.Value)
        {
            errors.Add(new(token.Index, "Expected option"));
            return null;
        }

        var kind = token.Kind == TokenKind.LongOption
            ? OptionKind.Long
            : OptionKind.Short;

        string name = kind == OptionKind.Long
            ? token.Value[2..]
            : token.Value[1..];

        IndexString? value = null;

        if (Peek() is Token peeked && peeked.Kind == TokenKind.Value)
        {
            Next();
            value = new(peeked.Index, peeked.Value);
        }

        return new(new OptionSignature(kind, name), value);
    }

    private Token? Peek() =>
        index < tokens.Length
            ? tokens[index]
            : null;

    private Token? Next() =>
        index < tokens.Length
            ? tokens[index++]
            : null;



    private readonly record struct Token(int Index, string Value, TokenKind Kind);

    private enum TokenKind
    {
        Value,
        LongOption,
        ShortOption
    }
}

internal readonly record struct ParseResult(
    IndexString[] Arguments,
    IReadOnlyDictionary<OptionSignature, Option> Options,
    IReadOnlyCollection<Error> Errors);

internal enum OptionKind
{
    Long,
    Short
}

internal readonly record struct Option(OptionSignature Signature, IndexString? Value)
{
    public override string ToString() =>
        Value is IndexString val
            ? $"{Signature} {val}"
            : Signature.ToString();
}

internal readonly record struct OptionSignature(OptionKind Kind, string Name)
{
    public override string ToString() =>
        $"{(Kind == OptionKind.Long ? "--" : "-")}{Name}";
}

internal readonly record struct IndexString(int Index, string Value)
{
    public override string ToString() => Value;
}

internal readonly record struct Error(int Index, string Message);
