using System;
using System.Threading.Tasks;

namespace NanopassSharp.Functional;

public static class TaskResultExtensions
{
    public static async Task<TResult> SwitchResultAsync<T, TResult>(this Task<Result<T>> source, Func<T, Task<TResult>> ifSuccess, Func<string, Task<TResult>> ifFailure) =>
        await (await source).SwitchAsync(ifSuccess, ifFailure);
    public static async Task<TResult> SwitchResultAsync<T, TResult>(this Task<Result<T>> source, Func<T, TResult> ifSuccess, Func<string, TResult> ifFailure) =>
        (await source).Switch(ifSuccess, ifFailure);

    /// <summary>
    /// Asynchronously returns the inner value of the result of a task evaluating to a
    /// <see cref="Result{T}"/> as either itself or the type's default value.
    /// </summary>
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

    public static async Task<Result<T>> NotNullResultAsync<T>(this Task<Result<T?>> result, string? error = null)
    where T : class =>
        (await result).Bind(v => v is not null ? new Result<T>(v) : error ?? "Inner result value was null");
    public static async Task<Result<T>> NotNullResultAsync<T>(this Task<Result<T?>> result, string? error = null)
    where T : struct =>
        (await result).Bind(v => v is not null ? new Result<T>(v.Value) : error ?? "Inner result value was null");
}
