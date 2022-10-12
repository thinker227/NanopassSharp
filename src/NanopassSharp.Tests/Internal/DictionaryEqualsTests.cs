using System.Collections.Generic;

namespace NanopassSharp.Internal.Tests;

public class DictionaryEqualsTests
{
    private static IEnumerable<object[]> ReturnsTrue_Data()
    {
        yield return new object[]
        {
            new Dictionary<TestData, string>(),
            new Dictionary<TestData, string>()
        };

        yield return new object[]
        {
            new Dictionary<TestData, string>
            {
                [new(2)] = "a",
                [new(1)] = "b",
                [new(3)] = "c",
            },
            new Dictionary<TestData, string>
            {
                [new(1)] = "b",
                [new(3)] = "c",
                [new(2)] = "a",
            }
        };
    }

    [MemberData(nameof(ReturnsTrue_Data))]
    [Theory]
    public void ReturnsTrue(IReadOnlyDictionary<TestData, string> a, IReadOnlyDictionary<TestData, string> b) =>
        a.DictionaryEquals(b).ShouldBeTrue();

    private static IEnumerable<object[]> WithComparer_ReturnsTrue_Data()
    {
        yield return new object[]
        {
            new Dictionary<TestData, string>(),
            new Dictionary<TestData, string>(),
        };

        {
            TestData a = new(2);
            TestData b = new(1);
            TestData c = new(3);

            yield return new object[]
            {
                new Dictionary<TestData, string>
                {
                    [a] = "a",
                    [b] = "b",
                    [c] = "c",
                },
                new Dictionary<TestData, string>
                {
                    [a] = "a",
                    [b] = "b",
                    [c] = "c",
                }
            };
        }
    }

    [MemberData(nameof(WithComparer_ReturnsTrue_Data))]
    [Theory]
    public void WithComparer_ReturnsTrue(IReadOnlyDictionary<TestData, string> a, IReadOnlyDictionary<TestData, string> b) =>
        a.DictionaryEquals(b, ReferenceEqualityComparer.Instance).ShouldBeTrue();

    private static IEnumerable<object[]> ReturnsFalse_Data()
    {
        yield return new object[]
        {
            new Dictionary<TestData, string>
            {
                [new(1)] = "a"
            },
            new Dictionary<TestData, string>()
        };

        yield return new object[]
        {
            new Dictionary<TestData, string>
            {
                [new(1)] = "a",
                [new(2)] = "b"
            },
            new Dictionary<TestData, string>
            {
                [new(1)] = "a"
            },
        };

        yield return new object[]
        {
            new Dictionary<TestData, string>
            {
                [new(1)] = "a",
                [new(2)] = "b"
            },
            new Dictionary<TestData, string>
            {
                [new(1)] = "a",
                [new(3)] = "b"
            },
        };

        yield return new object[]
        {
            new Dictionary<TestData, string>
            {
                [new(1)] = "a",
                [new(2)] = "b",
                [new(3)] = "c"
            },
            new Dictionary<TestData, string>
            {
                [new(1)] = "a",
                [new(2)] = "d",
                [new(3)] = "c"
            },
        };
    }

    [MemberData(nameof(ReturnsFalse_Data))]
    [Theory]
    public void ReturnsFalse(IReadOnlyDictionary<TestData, string> a, IReadOnlyDictionary<TestData, string> b) =>
        a.DictionaryEquals(b).ShouldBeFalse();
}

public record class TestData(int Data);
