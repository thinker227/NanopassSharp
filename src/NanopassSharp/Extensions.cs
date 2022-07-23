using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanopassSharp;

public static class Extensions {

	public static IEnumerable<(T first, T second)> WithNext<T>(this IEnumerable<T> source) {
		T? prev = default;
		bool hasPrev = true;
		
		foreach (var element in source) {
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

	public static async Task<U> Then<T, U>(this Task<T> task, Func<T, Task<U>> f) {
		var result = await task;
		return await f(result);
	}
	public static async ValueTask<U> Then<T, U>(this ValueTask<T> task, Func<T, Task<U>> f) {
		var result = await task;
		return await f(result);
	}

}
