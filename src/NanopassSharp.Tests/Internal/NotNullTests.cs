using System;
using System.Collections.Generic;

namespace NanopassSharp.Internal.Tests;

public class NotNullTests
{
    private static IEnumerable<object[]> ReturnsFilteredSequence_Data()
    {
        yield return new object[]
        {
            Array.Empty<string?>(),
            Array.Empty<string?>()
        };

        yield return new object[]
        {
            new[]
            {
                "a",
                "b",
                null,
                "c",
                null,
                null
            },
            new[]
            {
                "a",
                "b",
                "c"
            }
        };

        yield return new object[]
        {
            new[]
            {
                "a",
                "b",
                "c"
            },
            new[]
            {
                "a",
                "b",
                "c"
            }
        };

        yield return new object[]
        {
            new string?[]
            {
                null,
                null,
                null
            },
            Array.Empty<string?>()
        };
    }

    [MemberData(nameof(ReturnsFilteredSequence_Data))]
    [Theory]
    public void ReturnsFilteredSequence(IEnumerable<string?> xs, IEnumerable<string?> expected) =>
        xs.NotNull().ShouldBe(expected);
}
