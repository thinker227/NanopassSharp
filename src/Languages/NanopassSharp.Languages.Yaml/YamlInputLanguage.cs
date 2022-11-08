using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using NanopassSharp.Languages.Yaml.Models;

namespace NanopassSharp.Languages.Yaml;

public sealed class YamlInputLanguage : IInputLanguage
{
    public Task<PassSequence> GeneratePassSequenceAsync(InputContext context, CancellationToken cancellationToken)
    {
        var deserializer = GetDeserializer();
        var passes = deserializer.Deserialize<List<PassModel>>(context.Text);
    }

    private static IDeserializer GetDeserializer() => new DeserializerBuilder()
        .WithNamingConvention(HyphenatedNamingConvention.Instance)
        .Build();
}
