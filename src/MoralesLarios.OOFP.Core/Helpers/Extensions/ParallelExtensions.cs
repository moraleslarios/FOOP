// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers.Extensions;
/// <summary>
/// Métodos de extensión que elevan un valor sincrónico al mundo asíncrono envolviéndolo en una <see cref="Task{TResult}"/> ya completada, sin consumir ningún hilo.
/// </summary>
public static class ParallelExtensions
{


    /// <summary>
    /// Ejecuta <c>To</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>
    /// Tarea que produce el valor de tipo <c>T</c> obtenido por la operación.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<T> ToAsync<T>(this T value) => Task.FromResult(value);




}

