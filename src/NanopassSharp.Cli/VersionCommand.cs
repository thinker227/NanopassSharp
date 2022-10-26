using Spectre.Console;

internal class VersionCommand
{
    public VersionCommand()
    {
    }

    internal int Execute()
    {
        AnsiConsole.WriteLine("Version message");
        return 0;
    }
}
