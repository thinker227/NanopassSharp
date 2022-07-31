using NanopassSharp.Cli;
using Spectre.Console.Cli;

CommandApp<RunCommand> app = new();
app.Configure(config => config
    .SetApplicationName("Nanopass#")
    .SetApplicationVersion("1.0.0"));
return await app.RunAsync(args);
