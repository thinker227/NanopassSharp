using NanopassSharp.Cli;
using Spectre.Console;
using Spectre.Console.Cli;

CommandApp<RunCommand> app = new();

app.Configure(config =>
{
    config.SetApplicationName("nanopass-sharp");
    config.SetApplicationVersion("1.0.0");

    config.AddExample(new[] { "csharp", "./passes.yaml" });
    config.AddExample(new[] { "csharp", "./src/passes.txt", "-o ./src/MyProject", "-i json" });

    config.CaseSensitivity(CaseSensitivity.None);

    config.SetExceptionHandler(ExceptionHandler.Handle);
});

return await app.RunAsync(args);
