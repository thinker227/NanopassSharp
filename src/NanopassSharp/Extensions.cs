using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace NanopassSharp;

public static class Extensions
{
    public static IEnumerable<(T first, T second)> WithNext<T>(this IEnumerable<T> source)
    {
        T? prev = default;
        bool hasPrev = true;

        foreach (var element in source)
        {
            if (hasPrev) yield return (prev!, element);

            prev = element;
            hasPrev = true;
        }
    }

    /// <summary>
    /// Dumb extension because
    /// <see cref="Enumerable.ToHashSet{TSource}(IEnumerable{TSource})"/> and
    /// <see cref="MoreLinq.MoreEnumerable.ToHashSet{TSource}(IEnumerable{TSource})"/>
    /// conflict.
    /// </summary>
    public static HashSet<T> ToHashSet_<T>(this IEnumerable<T> source) =>
        Enumerable.ToHashSet(source);
    /// <summary>
    /// Dumb extension because
    /// <see cref="Enumerable.Prepend{TSource}(IEnumerable{TSource}, TSource)"/> and
    /// <see cref="MoreLinq.MoreEnumerable.Prepend{TSource}(IEnumerable{TSource}, TSource)"/>
    /// conflict.
    /// </summary>
    public static IEnumerable<T> Prepend_<T>(this IEnumerable<T> source, T element) =>
        Enumerable.Prepend(source, element);

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source) =>
        source.Where(s => s is not null)!;

    public static async Task<U> Then<T, U>(this Task<T> task, Func<T, Task<U>> f)
    {
        var result = await task;
        return await f(result);
    }
    public static async ValueTask<U> Then<T, U>(this ValueTask<T> task, Func<T, Task<U>> f)
    {
        var result = await task;
        return await f(result);
    }

    public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> other) =>
        source.DictionaryEquals(other, EqualityComparer<TValue>.Default);
    public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> other, IEqualityComparer<TValue> valueComparer) =>
        DictionaryEquals(new ReadOnlyDictionaryAdapter<TKey, TValue>(source), new ReadOnlyDictionaryAdapter<TKey, TValue>(other), valueComparer);
    public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> source, IReadOnlyDictionary<TKey, TValue> other) =>
        source.DictionaryEquals(other, EqualityComparer<TValue>.Default);
    public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> source, IReadOnlyDictionary<TKey, TValue> other, IEqualityComparer<TValue> valueComparer) =>
        DictionaryEquals(new ReadOnlyDictionaryAdapter<TKey, TValue>(source), other, valueComparer);
    public static bool DictionaryEquals<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IDictionary<TKey, TValue> other) =>
        source.DictionaryEquals(other, EqualityComparer<TValue>.Default);
    public static bool DictionaryEquals<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IDictionary<TKey, TValue> other, IEqualityComparer<TValue> valueComparer) =>
        DictionaryEquals(source, new ReadOnlyDictionaryAdapter<TKey, TValue>(other), valueComparer);
    public static bool DictionaryEquals<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IReadOnlyDictionary<TKey, TValue> other) =>
        source.DictionaryEquals(other, EqualityComparer<TValue>.Default);
    public static bool DictionaryEquals<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IReadOnlyDictionary<TKey, TValue> other, IEqualityComparer<TValue> valueComparer)
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
    private sealed class ReadOnlyDictionaryAdapter<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> dictionary;

        public TValue this[TKey key] => dictionary[key];
        public IEnumerable<TKey> Keys => dictionary.Keys;
        public IEnumerable<TValue> Values => dictionary.Values;
        public int Count => dictionary.Count;

        public ReadOnlyDictionaryAdapter(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => dictionary.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static bool SetEquals<T>(this ISet<T> source, ISet<T> other) =>
        source.Count == other.Count && source.Overlaps(other);
}
