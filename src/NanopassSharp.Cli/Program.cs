using System.CommandLine.Parsing;
using NanopassSharp.Cli;

var builder = CommandLineConfigurer.GetRootBuilder();
var parser = builder.Build();

return await parser.InvokeAsync(args);
