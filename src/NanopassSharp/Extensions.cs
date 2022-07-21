using System;
using System.Collections.Generic;
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

	public static async Task<U> Then<T, U>(this Task<T> task, Func<T, Task<U>> f) {
		var result = await task;
		return await f(result);
	}
	public static async ValueTask<U> Then<T, U>(this ValueTask<T> task, Func<T, Task<U>> f) {
		var result = await task;
		return await f(result);
	}

}
