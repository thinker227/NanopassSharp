using System.Threading.Tasks;

namespace NanopassSharp;

/// <summary>
/// An input format for a pass sequence.
/// </summary>
public interface IPassInputFormat
{
    /// <summary>
    /// Generates a <see cref="PassSequence"/> from the current input.
    /// </summary>
    /// <param name="context">The context for the initialization of the current execution.</param>
    /// <returns>A <see cref="PassSequence"/> retrieved from the current context.</returns>
    Task<PassSequence> GeneratePassSequenceAsync(InitializationContext context);
}
