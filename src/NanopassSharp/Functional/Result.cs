using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NanopassSharp.Functional;

/// <summary>
/// Represents either a result value or a string error.
/// </summary>
public readonly struct Result<T> {

	/// <summary>
	/// The result value.
	/// </summary>
	public T? Value { get; }
	/// <summary>
	/// The error string.
	/// </summary>
	public string? Error { get; }

	/// <summary>
	/// Whether the object contains a value or an error.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Value))]
	[MemberNotNullWhen(false, nameof(Error))]
	public bool IsSuccess { get; }



	/// <summary>
	/// Initializes a new <see cref="Result{T}"/> instance.
	/// </summary>
	public Result() {
		Value = default;
		Error = "<empty error>";
		IsSuccess = false;
	}
	/// <summary>
	/// Initializes a new <see cref="Result{T}"/> instance.
	/// </summary>
	/// <param name="value">The value of the result.</param>
	public Result(T value) {
		Value = value;
		Error = null;
		IsSuccess = true;
	}
	/// <summary>
	/// Initializes a new <see cref="Result{T}"/> instance.
	/// </summary>
	/// <param name="error">The error string of the result.</param>
	public Result(string error) {
		Value = default;
		Error = error;
		IsSuccess = false;
	}



	/// <summary>
	/// Switches over the result and returns a value.
	/// </summary>
	/// <typeparam name="TResult">The return type of the switch.</typeparam>
	/// <param name="ifSuccess">The function to execute if the result is a success.</param>
	/// <param name="ifFailure">The function to execute if the result is a failure.</param>
	/// <returns>The return value of the switch.</returns>
	public TResult Switch<TResult>(Func<T, TResult> ifSuccess, Func<string, TResult> ifFailure) =>
		IsSuccess
			? ifSuccess(Value)
			: ifFailure(Error);
	/// <summary>
	/// Asynchronously switches over the result and returns a value.
	/// </summary>
	/// <typeparam name="TResult">The return type of the switch.</typeparam>
	/// <param name="ifSuccess">The function to execute if the result is a success.</param>
	/// <param name="ifFailure">The function to execute if the result is a failure.</param>
	/// <returns>The return value of the switch.</returns>
	public Task<TResult> SwitchAsync<TResult>(Func<T, Task<TResult>> ifSuccess, Func<string, Task<TResult>> ifFailure) =>
		IsSuccess
			? ifSuccess(Value)
			: ifFailure(Error);
	/// <summary>
	/// Switches over the result without returning a value.
	/// </summary>
	/// <param name="ifSuccess">The function to execute if the result is a success.</param>
	/// <param name="ifFailure">The function to execute if the result is a failure.</param>
	public void Switch(Action<T> ifSuccess, Action<string> ifFailure) {
		if (IsSuccess) ifSuccess(Value);
		else ifFailure(Error);
	}
	/// <summary>
	/// Asynchronously switches over the result without returning a value.
	/// </summary>
	/// <param name="ifSuccess">The function to execute if the result is a success.</param>
	/// <param name="ifFailure">The function to execute if the result is a failure.</param>
	public async Task SwitchAsync(Func<T, Task> ifSuccess, Func<string, Task> ifFailure) {
		if (IsSuccess) await ifSuccess(Value);
		else await ifFailure(Error);
	}
	/// <summary>
	/// Returns the inner value as either itself or the type's default value.
	/// </summary>
	public T? ToMaybeDefault() =>
		IsSuccess
			? Value
			: default;

	/// <summary>
	/// Maps the inner value of the result.
	/// </summary>
	/// <typeparam name="U">The type of the new value to map to.</typeparam>
	/// <param name="f">The transformer function to execute on the inner value.</param>
	/// <returns>A new <see cref="Result{T}"/> with the inner value mapped if the source result was a success.</returns>
	public Result<U> Map<U>(Func<T, U> f) =>
		IsSuccess
			? new(f(Value))
			: new(Error);
	/// <summary>
	/// Asynchronously maps the inner value of the result.
	/// </summary>
	/// <typeparam name="U">The type of the new value to map to.</typeparam>
	/// <param name="f">The transformer function to execute on the inner value.</param>
	/// <returns>A new <see cref="Result{T}"/> with the inner value mapped if the source result was a success.</returns>
	public async Task<Result<U>> MapAsync<U>(Func<T, Task<U>> f) =>
		IsSuccess
			? new(await f(Value))
			: new(Error);
	/// <summary>
	/// Binds the inner value of the result to a new <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to bind to.</typeparam>
	/// <param name="f">The transformer function returning a new result to execute on the inner value.</param>
	/// <returns>The <see cref="Result{T}"/> returned by <paramref name="f"/>.</returns>
	public Result<U> Bind<U>(Func<T, Result<U>> f) =>
		IsSuccess
			? f(Value)
			: new(Error);
	/// <summary>
	/// Asynchronously binds the inner value of the result to a new <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to bind to.</typeparam>
	/// <param name="f">The transformer function returning a new result to execute on the inner value.</param>
	/// <returns>The <see cref="Result{T}"/> returned by <paramref name="f"/>.</returns>
	public async Task<Result<U>> BindAsync<U>(Func<T, Task<Result<U>>> f) =>
		IsSuccess
			? await f(Value)
			: new(Error);



	public static implicit operator Result<T>(T value) =>
		new(value);
	public static implicit operator Result<T>(string error) =>
		new(error);

}

public static class Result {

	/// <summary>
	/// Creates a <see cref="Result{T}"/> with a successful state.
	/// </summary>
	/// <param name="value">The success value.</param>
	public static Result<T> Success<T>(T value) =>
		new(value);
	/// <summary>
	/// Creates a <see cref="Result{T}"/> with a failure state.
	/// </summary>
	public static Result<T> Failure<T>(string error) =>
		new(error);
	/// <summary>
	/// Creates a <see cref="Result{T}"/> from a nullable value depending on the value is null.
	/// </summary>
	/// <param name="value">The value of the result.</param>
	/// <param name="error">The error message is <paramref name="error"/> is null.</param>
	public static Result<T> CreateNotNull<T>(T? value, string? error = null, [CallerArgumentExpression(nameof(value))] string callerExpression = "") where T : class =>
		value is not null
			? new(value)
			: new(error ?? $"{callerExpression} was null");
	/// <summary>
	/// Creates a <see cref="Result{T}"/> from a nullable value depending on the value is null.
	/// </summary>
	/// <param name="value">The value of the result.</param>
	/// <param name="error">The error message is <paramref name="error"/> is null.</param>
	public static Result<T> CreateNotNull<T>(T? value, string? error = null, [CallerArgumentExpression(nameof(value))] string callerExpression = "") where T : struct =>
		value is not null
			? new(value.Value)
			: new(error ?? $"{callerExpression} was null");

}

public static class ResultExtensions {

	public static async Task<TResult> SwitchResultAsync<T, TResult>(this Task<Result<T>> source, Func<T, Task<TResult>> ifSuccess, Func<string, Task<TResult>> ifFailure) =>
		await (await source).SwitchAsync(ifSuccess, ifFailure);
	public static async Task<TResult> SwitchResultAsync<T, TResult>(this Task<Result<T>> source, Func<T, TResult> ifSuccess, Func<string, TResult> ifFailure) =>
		(await source).Switch(ifSuccess, ifFailure);
	
	public static async Task<T?> ToMaybeDefaultAsync<T>(this Task<Result<T>> source) =>
		(await source).ToMaybeDefault();

	/// <summary>
	/// Asynchronously maps the inner value of the result of a task evaluating to a <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to map to.</typeparam>
	/// <param name="f">The transformer function to execute on the inner value.</param>
	/// <returns>A new <see cref="Result{T}"/> with the inner value mapped if the source result was a success.</returns>
	public static async Task<Result<U>> MapResultAsync<T, U>(this Task<Result<T>> source, Func<T, Task<U>> f) =>
		await (await source).MapAsync(f);
	/// <summary>
	/// Asynchronously maps the inner value of the result of a value task evaluating to a <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to map to.</typeparam>
	/// <param name="f">The transformer function to execute on the inner value.</param>
	/// <returns>A new <see cref="Result{T}"/> with the inner value mapped if the source result was a success.</returns>
	public static async Task<Result<U>> MapResultAsync<T, U>(this ValueTask<Result<T>> source, Func<T, Task<U>> f) =>
		await (await source).MapAsync(f);
	/// <summary>
	/// Asynchronously maps the inner value of the result of a task evaluating to a <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to map to.</typeparam>
	/// <param name="f">The transformer function to execute on the inner value.</param>
	/// <returns>A new <see cref="Result{T}"/> with the inner value mapped if the source result was a success.</returns>
	public static async Task<Result<U>> MapResultAsync<T, U>(this Task<Result<T>> source, Func<T, U> f) =>
		(await source).Map(f);
	/// <summary>
	/// Asynchronously maps the inner value of the result of a value task evaluating to a <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to map to.</typeparam>
	/// <param name="f">The transformer function to execute on the inner value.</param>
	/// <returns>A new <see cref="Result{T}"/> with the inner value mapped if the source result was a success.</returns>
	public static async Task<Result<U>> MapResultAsync<T, U>(this ValueTask<Result<T>> source, Func<T, U> f) =>
		(await source).Map(f);
	/// <summary>
	/// Asynchronously binds the inner value of the result of a task evaluating to a <see cref="Result{T}"/> to a new <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to bind to.</typeparam>
	/// <param name="f">The transformer function returning a new result to execute on the inner value.</param>
	/// <returns>The <see cref="Result{T}"/> returned by <paramref name="f"/>.</returns>
	public static async Task<Result<U>> BindResultAsync<T, U>(this Task<Result<T>> source, Func<T, Task<Result<U>>> f) =>
		await (await source).BindAsync(f);
	/// <summary>
	/// Asynchronously binds the inner value of the result of a value task evaluating to a <see cref="Result{T}"/> to a new <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to bind to.</typeparam>
	/// <param name="f">The transformer function returning a new result to execute on the inner value.</param>
	/// <returns>The <see cref="Result{T}"/> returned by <paramref name="f"/>.</returns>
	public static async Task<Result<U>> BindResultAsync<T, U>(this ValueTask<Result<T>> source, Func<T, Task<Result<U>>> f) =>
		await (await source).BindAsync(f);
	/// <summary>
	/// Asynchronously binds the inner value of the result of a task evaluating to a <see cref="Result{T}"/> to a new <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to bind to.</typeparam>
	/// <param name="f">The transformer function returning a new result to execute on the inner value.</param>
	/// <returns>The <see cref="Result{T}"/> returned by <paramref name="f"/>.</returns>
	public static async Task<Result<U>> BindResultAsync<T, U>(this Task<Result<T>> source, Func<T, Result<U>> f) =>
		(await source).Bind(f);
	/// <summary>
	/// Asynchronously binds the inner value of the result of a value task evaluating to a <see cref="Result{T}"/> to a new <see cref="Result{T}"/>.
	/// </summary>
	/// <typeparam name="U">The type of the new value to bind to.</typeparam>
	/// <param name="f">The transformer function returning a new result to execute on the inner value.</param>
	/// <returns>The <see cref="Result{T}"/> returned by <paramref name="f"/>.</returns>
	public static async Task<Result<U>> BindResultAsync<T, U>(this ValueTask<Result<T>> source, Func<T, Result<U>> f) =>
		(await source).Bind(f);

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
	public static async Task<Result<T>> NotNullResultAsync<T>(this Task<Result<T?>> result, string? error = null)
	where T : class =>
		(await result).Bind(v => v is not null ? new Result<T>(v) : error ?? "Inner result value was null");
	public static async Task<Result<T>> NotNullResultAsync<T>(this Task<Result<T?>> result, string? error = null)
	where T : struct =>
		(await result).Bind(v => v is not null ? new Result<T>(v.Value) : error ?? "Inner result value was null");

	public static IEnumerable<T> WhereSuccessful<T>(this IEnumerable<Result<T>> source) =>
		source
			.Where(e => e.IsSuccess)
			.Select(e => e.Value!);

}
