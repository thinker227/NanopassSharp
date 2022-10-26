using Spectre.Console;

internal class HelpCommand
{
    public HelpCommand()
    {
    }

    internal int Execute()
    {
        AnsiConsole.WriteLine("Help message");
        return 0;
    }
}
