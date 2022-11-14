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
    public static string? ReadResourceAsString(string resourceLocation) =>
        ReadResourceAsString(resourceLocation, Assembly.GetCallingAssembly());

    /// <summary>
    /// Reads a resource as a string
    /// and throws an exception if the resource could not be read.
    /// </summary>
    /// <param name="resourceLocation">The location of the resource.</param>
    /// <returns>The resource at the location <paramref name="resourceLocation"/>
    /// within the calling assembly.</returns>
    /// <exception cref="IOException"></exception>
    public static string ReadResourceAsStringOrThrow(string resourceLocation) =>
        ReadResourceAsString(resourceLocation, Assembly.GetCallingAssembly())
            ?? throw new IOException($"Failed to read manifest resource '{resourceLocation}'.");

    /// <summary>
    /// Reads a resource as a string.
    /// </summary>
    /// <param name="resourceLocation">The location of the resource.</param>
    /// <param name="assembly">The assembly in which the resource is located.</param>
    /// <returns>The resource at the location <paramref name="resourceLocation"/>
    /// within <paramref name="assembly"/>, or <see langword="null"/> if the resource
    /// could not be read.</returns>
    public static string? ReadResourceAsString(string resourceLocation, Assembly assembly)
    {
        using var resourceStream = assembly.GetManifestResourceStream(resourceLocation);
        if (resourceStream is null) return null;

        StreamReader reader = new(resourceStream);

        return reader.ReadToEnd();
    }
}
