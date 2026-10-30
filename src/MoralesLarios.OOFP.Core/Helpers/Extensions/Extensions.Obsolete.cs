// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace MoralesLarios.OOFP.Helpers.Extensions;

public static partial class Extensions
{

    /// <inheritdoc cref="NullableExtensions.ToNullable{T}(T)"/>
    [Obsolete("Moved to NullableExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T? ToNullable<T>(T source)
        where T : struct
            => NullableExtensions.ToNullable(source);


    /// <inheritdoc cref="ExceptionMetadataExtensions.AppendExDetails(Dictionary{string, object}, Exception)"/>
    [Obsolete("Moved to ExceptionMetadataExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Dictionary<string, object> AppendExDetails(Dictionary<string, object> source, Exception ex)
        => ExceptionMetadataExtensions.AppendExDetails(source, ex);


    /// <inheritdoc cref="MutationExtensions.With{T}(T, Action{T}[])"/>
    [Obsolete("Moved to MutationExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T With<T>(T source, params Action<T>[] changes)
        where T : class
            => MutationExtensions.With(source, changes);


    /// <inheritdoc cref="MutationExtensions.WithAsync{T}(T, Action{T}[])"/>
    [Obsolete("Moved to MutationExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<T> WithAsync<T>(T source, params Action<T>[] changes)
        where T : class
            => MutationExtensions.WithAsync(source, changes);


    /// <inheritdoc cref="MutationExtensions.WithAsync{T}(Task{T}, Action{T}[])"/>
    [Obsolete("Moved to MutationExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<T> WithAsync<T>(Task<T> sourceAsync, params Action<T>[] changes)
        where T : class
            => MutationExtensions.WithAsync(sourceAsync, changes);


    /// <inheritdoc cref="TaskLiftExtensions.VoidToAsync{T}(T, Action{T})"/>
    [Obsolete("Moved to TaskLiftExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task VoidToAsync<T>(T source, Action<T> voidAction)
        => TaskLiftExtensions.VoidToAsync(source, voidAction);


    /// <inheritdoc cref="TaskLiftExtensions.ToFuncTask{T, TResult}(Func{T, TResult})"/>
    [Obsolete("Moved to TaskLiftExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Func<T, Task<TResult>> ToFuncTask<T, TResult>(Func<T, TResult> func)
        => TaskLiftExtensions.ToFuncTask(func);


    /// <inheritdoc cref="TaskLiftExtensions.ToFuncTask{TResult}(Func{MlErrorDetails, TResult})"/>
    [Obsolete("Moved to TaskLiftExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Func<MlErrorDetails, Task<TResult>> ToFuncTask<TResult>(Func<MlErrorDetails, TResult> func)
        => TaskLiftExtensions.ToFuncTask(func);


    /// <inheritdoc cref="TaskLiftExtensions.ToFuncTask{T}(Action{T})"/>
    [Obsolete("Moved to TaskLiftExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Func<T, Task> ToFuncTask<T>(Action<T> action)
        => TaskLiftExtensions.ToFuncTask(action);


    /// <inheritdoc cref="TaskLiftExtensions.ToFuncTask(Action{MlErrorDetails})"/>
    [Obsolete("Moved to TaskLiftExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Func<MlErrorDetails, Task> ToFuncTask(Action<MlErrorDetails> action)
        => TaskLiftExtensions.ToFuncTask(action);


    /// <inheritdoc cref="TaskLiftExtensions.ToFuncTask(Action)"/>
    [Obsolete("Moved to TaskLiftExtensions. Will be removed in 3.0.0.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Func<Task> ToFuncTask(Action action)
        => TaskLiftExtensions.ToFuncTask(action);

}
