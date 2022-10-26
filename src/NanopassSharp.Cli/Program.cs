using NanopassSharp.Cli;
using NanopassSharp.Cli.Input;

var parseResult = Parser.Parse(args);
var command = Command.Create(parseResult);

return command switch
{
    Command.Run run => await new RunCommand(run.Arguments).ExecuteAsync(),
    Command.Help => new HelpCommand().Execute(),
    Command.Version => new VersionCommand().Execute(),
    Command.Error error => new ErrorReporter(error.Errors).Report(),
    _ => 1
};
