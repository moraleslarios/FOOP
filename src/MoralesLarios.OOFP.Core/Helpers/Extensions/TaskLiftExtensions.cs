// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers.Extensions;

/// <summary>
/// Métodos de extensión que elevan delegados y acciones sincrónicas a su equivalente asíncrono
/// (<c>Func&lt;T, TResult&gt;</c> → <c>Func&lt;T, Task&lt;TResult&gt;&gt;</c>,
/// <c>Action&lt;T&gt;</c> → <c>Func&lt;T, Task&gt;</c>), para poder reutilizar código sincrónico
/// en las sobrecargas asíncronas de los combinadores del núcleo.
/// </summary>
/// <remarks>
/// Esta clase forma parte del troceado de la antigua clase <c>Extensions</c>, que acumulaba
/// familias de métodos sin relación entre sí. El espacio de nombres
/// <c>MoralesLarios.OOFP.Helpers.Extensions</c> <b>no cambia</b>, de modo que la invocación en
/// forma de extensión (<c>func.ToFuncTask()</c>) sigue resolviéndose igual: el compilador busca
/// los métodos de extensión en <b>todas</b> las clases estáticas del espacio de nombres
/// importado, no en una clase concreta.
/// <para>
/// Sólo la invocación en forma <b>estática</b> (<c>Extensions.ToFuncTask(func)</c>) queda
/// afectada; para ella se conservan puentes <c>[Obsolete]</c> en
/// <c>Helpers/Extensions/Extensions.Obsolete.cs</c>, que desaparecerán en la versión 3.0.0.
/// </para>
/// <para>
/// Las tareas que producen los delegados elevados <b>ya están completadas</b>: la elevación es
/// puramente sintáctica y no introduce ningún punto de suspensión real ni admite cancelación.
/// </para>
/// </remarks>
public static class TaskLiftExtensions
{

    /// <summary>
    /// Ejecuta una acción sobre el objeto de origen y devuelve una tarea ya completada.
    /// </summary>
    /// <typeparam name="T">Tipo del objeto de origen.</typeparam>
    /// <param name="source">Objeto de origen sobre el que se ejecuta la acción.</param>
    /// <param name="voidAction">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Tarea <b>ya completada</b> que representa la ejecución de la acción.</returns>
    public static Task VoidToAsync<T>(this T source, Action<T> voidAction)
    {
        voidAction(source);

        return Task.CompletedTask;
    }


    /// <summary>
    /// Eleva una función sincrónica a su equivalente asíncrono.
    /// </summary>
    /// <typeparam name="T">Tipo del argumento de la función.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto por la función.</typeparam>
    /// <param name="func">Función sincrónica a elevar.</param>
    /// <returns>Función equivalente que devuelve una tarea <b>ya completada</b> con el resultado.</returns>
    public static Func<T, Task<TResult>> ToFuncTask<T, TResult>(this Func<T, TResult> func) => x => func(x).ToAsync();


    /// <summary>
    /// Eleva una función sincrónica sobre los detalles de error a su equivalente asíncrono.
    /// </summary>
    /// <typeparam name="TResult">Tipo del valor devuelto por la función.</typeparam>
    /// <param name="func">Función sincrónica a elevar.</param>
    /// <returns>Función equivalente que devuelve una tarea <b>ya completada</b> con el resultado.</returns>
    public static Func<MlErrorDetails, Task<TResult>> ToFuncTask<TResult>(this Func<MlErrorDetails, TResult> func) => errorsDetails => func(errorsDetails).ToAsync();


    /// <summary>
    /// Eleva una acción sincrónica a su equivalente asíncrono.
    /// </summary>
    /// <typeparam name="T">Tipo del argumento de la acción.</typeparam>
    /// <param name="action">Acción sincrónica a elevar.</param>
    /// <returns>Función equivalente que devuelve una tarea <b>ya completada</b>.</returns>
    public static Func<T, Task> ToFuncTask<T>(this Action<T> action)
    {
        return x =>
        {
            action(x);
            return Task.CompletedTask;
        };
    }


    /// <summary>
    /// Eleva una acción sincrónica sobre los detalles de error a su equivalente asíncrono.
    /// </summary>
    /// <param name="action">Acción sincrónica a elevar.</param>
    /// <returns>Función equivalente que devuelve una tarea <b>ya completada</b>.</returns>
    public static Func<MlErrorDetails, Task> ToFuncTask(this Action<MlErrorDetails> action)
    {
        return errorsDetails =>
        {
            action(errorsDetails);
            return Task.CompletedTask;
        };
    }


    /// <summary>
    /// Eleva una acción sincrónica sin argumentos a su equivalente asíncrono.
    /// </summary>
    /// <param name="action">Acción sincrónica a elevar.</param>
    /// <returns>Función equivalente que devuelve una tarea <b>ya completada</b>.</returns>
    public static Func<Task> ToFuncTask(this Action action)
    {
        return () =>
        {
            action();
            return Task.CompletedTask;
        };
    }

}
