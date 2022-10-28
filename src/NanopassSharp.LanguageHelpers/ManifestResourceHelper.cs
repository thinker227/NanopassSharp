using System.IO;
using System.Reflection;

namespace NanopassSharp.LanguageHelpers;

/// <summary>
/// Helper methods for handling manifest resources.
/// </summary>
public static class ManifestResourceHelper
{
    /// <summary>
    /// Reads a resource as a string.
    /// </summary>
    /// <param name="resourceLocation">The location of the resource.</param>
    /// <returns>The resource at the location <paramref name="resourceLocation"/>
    /// within the calling assembly, or <see langword="null"/> if the resource
    /// could not be read.</returns>
    public static string? ReadResourceAsString(string resourceLocation)
    {
        var assembly = Assembly.GetCallingAssembly();

        using var resourceStream = assembly.GetManifestResourceStream(resourceLocation);
        if (resourceStream is null) return null;

        StreamReader reader = new(resourceStream);

        return reader.ReadToEnd();
    }

    /// <summary>
    /// Reads a resource as a string
    /// and throws an exception if the resource could not be read.
    /// </summary>
    /// <param name="resourceLocation">The location of the resource.</param>
    /// <returns>The resource at the location <paramref name="resourceLocation"/>
    /// within the calling assembly.</returns>
    /// <exception cref="IOException"></exception>
    public static string ReadResourceAsStringOrThrow(string resourceLocation) =>
        ReadResourceAsString(resourceLocation)
            ?? throw new IOException($"Failed to read manifest resource '{resourceLocation}'.");
}
