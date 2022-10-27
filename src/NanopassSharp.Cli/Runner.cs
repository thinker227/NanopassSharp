using System.Threading.Tasks;

namespace NanopassSharp.Cli;

internal sealed class Runner
{
    private readonly Options options;



    public Runner(Options options)
    {
        this.options = options;
    }



    public Task RunAsync()
    {
        return Task.CompletedTask;
    }
}
