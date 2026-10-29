// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.Helpers.Extensions;
/// <summary>
/// Métodos de extensión de uso general del núcleo: conversión a nullable, clonado con mutaciones (<c>With</c>), adaptación de delegados sincrónicos a asíncronos y volcado de los datos de una excepción a un diccionario de metadatos.
/// </summary>
public static class Extensions
{


    /// <summary>
    /// Ejecuta <c>ToNullable</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>T?</c> obtenido por la operación.</returns>
    public static T? ToNullable<T>(this T source)
        where T : struct
            => source;


    /// <summary>
    /// Añade el elemento al final de <c>ExDetails</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Valor de tipo <c>Dictionary&lt;string, object&gt;</c> obtenido por la operación.</returns>
    public static Dictionary<string, object> AppendExDetails(this Dictionary<string, object> source, Exception ex)
    {
        var exKeys = source.Keys.Where(x => x.StartsWith(Constants.ExceptionMetadataKey)).ToList();

        var exKey = exKeys.Any() ? $"{Constants.ExceptionMetadataKey}{exKeys.Count + 1}" : Constants.ExceptionMetadataKey;

        var result = source.ToDictionary(x => x.Key, x => x.Value);

        result.Add(exKey, ex);

        return result;
    }





    /// <summary>
    /// Ejecuta <c>With</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="changes">Acción a ejecutar.</param>
    /// <returns>Valor de tipo <c>T</c> obtenido por la operación.</returns>
    public static T With<T>(this T source, params Action<T>[] changes) 
        where T : class
    {
        foreach (var change in changes)
        {
            change(source);
        }

        return source;
    }


    /// <summary>
    /// Ejecuta <c>With</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="changes">Acción a ejecutar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>T</c> obtenido por la operación.</returns>
    public static Task<T> WithAsync<T>(this T source, params Action<T>[] changes) 
        where T : class
        => source.With(changes).ToAsync();

    /// <summary>
    /// Ejecuta <c>With</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="changes">Acción a ejecutar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>T</c> obtenido por la operación.</returns>
    public static async Task<T> WithAsync<T>(this Task<T> sourceAsync, params Action<T>[] changes) 
        where T : class
        => await (await sourceAsync).WithAsync(changes);



    /// <summary>
    /// Ejecuta <c>VoidTo</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="voidAction">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    public static Task VoidToAsync<T>(this T source, Action<T> voidAction)
    {
        voidAction(source);

        return Task.CompletedTask;
    }


    /// <summary>
    /// Ejecuta <c>ToFuncTask</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Valor de tipo <c>Func&lt;T, Task&lt;TResult&gt;&gt;</c> obtenido por la operación.</returns>
    public static Func<T, Task<TResult>> ToFuncTask<T, TResult>(this Func<T, TResult> func) => x => func(x).ToAsync();

    /// <summary>
    /// Ejecuta <c>ToFuncTask</c>.
    /// </summary>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Valor de tipo <c>Func&lt;MlErrorDetails, Task&lt;TResult&gt;&gt;</c> obtenido por la operación.</returns>
    public static Func<MlErrorDetails, Task<TResult>> ToFuncTask<TResult>(this Func<MlErrorDetails, TResult> func) => errorsDetails => func(errorsDetails).ToAsync();

    /// <summary>
    /// Ejecuta <c>ToFuncTask</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Valor de tipo <c>Func&lt;T, Task&gt;</c> obtenido por la operación.</returns>
    public static Func<T, Task> ToFuncTask<T>(this Action<T> action)
    {
        return x =>
        {
            action(x);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// Ejecuta <c>ToFuncTask</c>.
    /// </summary>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Valor de tipo <c>Func&lt;MlErrorDetails, Task&gt;</c> obtenido por la operación.</returns>
    public static Func<MlErrorDetails, Task> ToFuncTask(this Action<MlErrorDetails> action)
    {
        return errorsDetails =>
        {
            action(errorsDetails);
            return Task.CompletedTask;
        };
    }


    /// <summary>
    /// Ejecuta <c>ToFuncTask</c>.
    /// </summary>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Valor de tipo <c>Func&lt;Task&gt;</c> obtenido por la operación.</returns>
    public static Func<Task> ToFuncTask(this Action action)
    {
        return () =>
        {
            action();
            return Task.CompletedTask;
        };
    }



}

