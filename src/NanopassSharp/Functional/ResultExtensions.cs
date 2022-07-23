using System;

namespace NanopassSharp.Functional;
public static class ResultExtensions {

	/// <summary>
	/// Returns the inner value as either itself or the type's default value.
	/// </summary>
	public static T? ToMaybeDefault<T>(this Result<T> source) =>
		source.IsSuccess
			? source.Value
			: default;

	/// <summary>
	/// Returns a <see cref="Result{T}"/> with a value depending on whether the inner value
	/// of the source is <see langword="null"/>.
	/// </summary>
	/// <param name="error">The error message is the inner value is null.</param>
	public static Result<T> NotNull<T>(this Result<T?> result, string? error = null)
	where T : class =>
		result.Bind(v => v is not null ? new Result<T>(v) : error ?? "Inner result value was null");
	/// <summary>
	/// Returns a <see cref="Result{T}"/> with a value depending on whether the inner value
	/// of the source is <see langword="null"/>.
	/// </summary>
	/// <param name="error">The error message is the inner value is null.</param>
	public static Result<T> NotNull<T>(this Result<T?> result, string? error = null)
	where T : struct =>
		result.Bind(v => v is not null ? new Result<T>(v.Value) : error ?? "Inner result value was null");

	/// <summary>
	/// Applies the inner function of a <see cref="Result{T}"/>
	/// to the inner value of another <see cref="Result{T}"/>.
	/// </summary>
	/// <param name="result">The result which inner value to apply the function to.</param>
	public static Result<U> Apply<T, U>(this Result<Func<T, U>> source, Result<T> result) =>
		source.Bind(f =>
			result.Bind(x =>
				new Result<U>(f(x))
			)
		);

}
