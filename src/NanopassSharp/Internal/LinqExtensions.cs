using System.Collections.Generic;
using System.Linq;

namespace NanopassSharp.Internal;

internal static class LinqExtensions
{
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source) => source.OfType<T>();

    public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> other) =>
        source.DictionaryEquals(other, EqualityComparer<TValue>.Default);

    public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> other, IEqualityComparer<TValue> valueComparer)
    {
        if (source.Count != other.Count) return false;

        foreach (var (key, sourceValue) in source)
        {
            if (!other.ContainsKey(key)) return false;

            var otherValue = other[key];
            if (!valueComparer.Equals(sourceValue, otherValue)) return false;
        }

        return true;
    }
}
