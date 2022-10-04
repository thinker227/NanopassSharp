﻿using System.Threading.Tasks;

namespace NanopassSharp;

/// <summary>
/// An input format for a pass sequence.
/// </summary>
public interface IInputLanguage
{
    /// <summary>
    /// Generates a <see cref="PassSequence"/> from an input.
    /// </summary>
    /// <param name="context">The context for the input.</param>
    /// <returns>A <see cref="PassSequence"/> retrieved from the current input.</returns>
    Task<PassSequence> GeneratePassSequenceAsync(InputContext context);
}
