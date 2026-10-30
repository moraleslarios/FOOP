// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;

/// <summary>
/// Métodos de extensión para operar con colecciones y resultados: recorridos (<c>Traverse</c>), secuenciación, particionado de elementos válidos y fallidos y agregación de errores.
/// </summary>
public static partial class MlResultCollections
{


    /// <summary>
    /// Ejecuta <c>Traverse</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<TResult>> Traverse<T, TResult>(this IEnumerable<T>             source,
                                                                             Func<T, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                var partialData = x.Select(completeFuncTransform).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>Traverse</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<TResult>> Traverse<T, TResult>(this IEnumerable<T>                  source,
                                                                             Func<T, int, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                var partialData = x.Select(completeFuncTransform).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }





    /// <summary>
    /// Ejecuta <c>Traverse</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                        Func<T, MlResult<TResult>> completeFuncTransform)
        => source.Traverse(completeFuncTransform).ToAsync();

    /// <summary>
    /// Ejecuta <c>Traverse</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this IEnumerable<T>                  source,
                                                                                        Func<T, int, MlResult<TResult>> completeFuncTransform)
        => source.Traverse(completeFuncTransform).ToAsync();


    /// <summary>
    /// Ejecuta <c>Traverse</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                              Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<TResult>>();
                                
                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>Traverse</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
                                                                                              Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<TResult>>();
                                
                                foreach (var item in colec.Select((value, index) => (value, index)))
                                {
                                    var funcResult = await completeFuncTransformAsync(item.value, item.index);
                                    partialData.Add(funcResult);
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }



    /// <summary>
    /// Ejecuta <c>Traverse</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                              Func<T, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).TraverseAsync(completeFuncTransform);


    /// <summary>
    /// Ejecuta <c>Traverse</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                              Func<T, int, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).TraverseAsync(completeFuncTransform);


    /// <summary>
    /// Ejecuta <c>Traverse</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                              Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<TResult>>();

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>Traverse</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                              Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<TResult>>();
                                
                                int index = 0;
                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item, index);
                                    partialData.Add(funcResult);
                                    index++;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<TResult>> TraverseWhile<T, TResult>(this IEnumerable<T>             source,
                                                                                  Func<T, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                List<MlResult<TResult>> partialData = [];

                                foreach (var item in x)
                                {
                                    var funcResult = completeFuncTransform(item);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<TResult>> TraverseWhile<T, TResult>(this IEnumerable<T>                  source,
                                                                                  Func<T, int, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                List<MlResult<TResult>> partialData = [];

                                int index = 0;
                                foreach (var item in x)
                                {
                                    var funcResult = completeFuncTransform(item, index);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                    index++;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                             Func<T, MlResult<TResult>> completeFuncTransform)
        => source.TraverseWhile(completeFuncTransform).ToAsync();


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                             Func<T, int, MlResult<TResult>> completeFuncTransform)
        => source.TraverseWhile(completeFuncTransform).ToAsync();



    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                   Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<TResult>>();

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
                                                                                                   Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<TResult>>();

                                int index = 0;

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item, index);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                    index++;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                   Func<T, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).TraverseWhileAsync(completeFuncTransform);


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                   Func<T, int, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).TraverseWhileAsync(completeFuncTransform);


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                   Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<TResult>>();

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseWhile</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                   Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<TResult>>();
                                int index = 0;  

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item, index);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                    index++;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseParallel</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseParallelAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                      Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                List<Task<MlResult<TResult>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>TraverseParallel</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseParallelAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
                                                                                                      Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                List<Task<MlResult<TResult>>> tasks = colec.Select((item, index) => completeFuncTransformAsync(item, index)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseParallel</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseParallelAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                      Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {

                                List<Task<MlResult<TResult>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseParallel</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<TResult>>> TraverseParallelAsync<T, TResult>(this IEnumerable<T>                        source,
                                                                                                      Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {

                                List<Task<MlResult<TResult>>> tasks = colec.Select((item, index) => completeFuncTransformAsync(item, index)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }



    /// <summary>
    /// Ejecuta <c>TraverseSplit</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)> TraverseSplit<T, TResult>(this IEnumerable<T>             source,
                                                                                                                                   Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransform, "completeFuncTransform cannot be null")
                            .Map (x =>
                            {
                                var partialData = source.Where(z => z is not null)
                                                        .Select(z => (z, completeFuncTransform(z))).ToList();

                                var partialResult = (valids: partialData.Where(x => x.Item2.IsSuccess).ToDictionary(x => x.Item1, x => x.Item2.GetValueOrThrow()),
                                                     fails : partialData.Where(x => x.Item2.IsFailure ).ToDictionary(x => x.Item1, x => x.Item2.GetFailureDetailsOrThrow()));
                                return partialResult;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseSplit</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> TraverseSplitAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                                                                              Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull
        => source.TraverseSplit(completeFuncTransform).ToAsync();


    /// <summary>
    /// Ejecuta <c>TraverseSplit</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> TraverseSplitAsync<T, TResult>(this Task<IEnumerable<T>>       sourceAsync,
                                                                                                                                                    Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull
        => await (await sourceAsync).TraverseSplitAsync(completeFuncTransform);


    /// <summary>
    /// Ejecuta <c>TraverseSplit</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> TraverseSplitAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                                                              Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where T : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransformAsync, "completeFuncTransformAsync cannot be null")
                            .MapAsync(async x =>
                            {
                                var partialData = new List<(T item, MlResult<TResult> result)>();

                                foreach (var item in source.Where(z => z is not null))
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add((item, funcResult));
                                }

                                var partialResult = (valids: partialData.Where(x => x.result.IsSuccess).ToDictionary(x => x.item, x => x.result.GetValueOrThrow()),
                                                     fails : partialData.Where(x => x.result.IsFailure ).ToDictionary(x => x.item, x => x.result.GetFailureDetailsOrThrow()));

                                return partialResult;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseSplit</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> TraverseSplitAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                                                                    Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where T : notnull
        => await (await sourceAsync).TraverseSplitAsync(completeFuncTransformAsync);




    /// <summary>
    /// Ejecuta <c>TraverseSplitWithKeys</c>.
    /// </summary>
    /// <typeparam name="TKey">Tipo de la clave de la entidad.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)> TraverseSplitWithKeys<TKey, TResult>(this IEnumerable<TKey> source,
                                                                                                                                                                              Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransform, "completeFuncTransform cannot be null")
                                .Map(x =>
                                {
                                    var partialData = source.Where(z => z is not null)
                                                            .Select(z => (z, completeFuncTransform(z))).ToList();
                                    var partialResult = (valids: partialData.Where(x => x.Item2.IsSuccess)
                                                                            .Select(x => new TraverseSplitSuccess<TKey, TResult>(x.Item1, x.Item2.GetValueOrThrow())),
                                                         fails: partialData.Where(x => x.Item2.IsFailure)
                                                                            .Select(x => new TraverseSplitFailure<TKey>(x.Item1, x.Item2.GetFailureDetailsOrThrow())));
                                    return partialResult;
                                });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseSplitWithKeys</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TKey">Tipo de la clave de la entidad.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)>> TraverseSplitWithKeysAsync<TKey, TResult>(this IEnumerable<TKey>             source,
                                                                                                                                                                                         Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
        => source.TraverseSplitWithKeys(completeFuncTransform).ToAsync();


    /// <summary>
    /// Ejecuta <c>TraverseSplitWithKeys</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TKey">Tipo de la clave de la entidad.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransform">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)>> TraverseSplitWithKeysAsync<TKey, TResult>(this Task<IEnumerable<TKey>>       sourceAsync,
                                                                                                                                                                                               Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
        => await (await sourceAsync).TraverseSplitWithKeysAsync(completeFuncTransform);


    /// <summary>
    /// Ejecuta <c>TraverseSplitWithKeys</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TKey">Tipo de la clave de la entidad.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)>> TraverseSplitWithKeysAsync<TKey, TResult>(this IEnumerable<TKey>                   source,
                                                                                                                                                                                         Func<TKey, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where TKey : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransformAsync, "completeFuncTransformAsync cannot be null")
                            .MapAsync(async x =>
                            {
                                var partialData = new List<(TKey item, MlResult<TResult> result)>();

                                foreach (var item in source.Where(z => z is not null))
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add((item, funcResult));
                                }

                                var partialResult = (valids: partialData.Where(x => x.result.IsSuccess)
                                                                        .Select(x => new TraverseSplitSuccess<TKey, TResult>(x.item, x.result.GetValueOrThrow())),
                                                     fails: partialData.Where(x => x.result.IsFailure)
                                                                        .Select(x => new TraverseSplitFailure<TKey>(x.item, x.result.GetFailureDetailsOrThrow())));

                                return partialResult;
                            });
        return result;
    }


    /// <summary>
    /// Ejecuta <c>TraverseSplitWithKeys</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TKey">Tipo de la clave de la entidad.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncTransformAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)>> TraverseSplitWithKeysAsync<TKey, TResult>(this Task<IEnumerable<TKey>>             sourceAsync,
                                                                                                                                                                                               Func<TKey, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where TKey : notnull
        => await (await sourceAsync).TraverseSplitWithKeysAsync(completeFuncTransformAsync);






    /// <summary>
    /// Combina los resultados de <c>FailureErrors</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> MergeFailureErrors<T>(this IEnumerable<MlResult<T>> source)
    {
        var partialResult = source.Where(x => x.IsFailure).ToList();

        if ( ! partialResult.Any()) return MlResult<IEnumerable<T>>.Fail("No elements found in failed state to merge");

        MlErrorDetails result = partialResult.First().FailureDetails;

        foreach (var item in partialResult.Skip(1))
        {
            result = result.Merge(item.FailureDetails);
        }

        return result;
    }


    /// <summary>
    /// Combina los resultados de <c>FailureErrors</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<IEnumerable<T>>> MergeFailureErrorsAsync<T>(this IEnumerable<MlResult<T>> source)
        => source.MergeFailureErrors().ToAsync();

    /// <summary>
    /// Combina los resultados de <c>FailureErrors</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<T>>> MergeFailureErrorsAsync<T>(this Task<IEnumerable<MlResult<T>>> sourceAsync)
        => await (await sourceAsync).MergeFailureErrorsAsync();




    /// <summary>
    /// Combina los resultados de <c>ErrorsIfExists</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> MergeErrorsIfExists<T>(this IEnumerable<MlResult<T>> source)
    {
        var partialResult = source.Where(x => x.IsFailure).ToList();

        if ( ! partialResult.Any())
        {
            return MlResult<IEnumerable<T>>.Valid(source.Select(x => x.GetValueOrThrow()));
        }

        MlErrorDetails result = partialResult.First().FailureDetails;

        foreach (var item in partialResult.Skip(1))
        {
            result = result.Merge(item.FailureDetails);
        }

        return result;

    }

    /// <summary>
    /// Combina los resultados de <c>ErrorsIfExists</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<IEnumerable<T>>> MergeErrorsIfExistsAsync<T>(this IEnumerable<MlResult<T>> source)
        => source.MergeErrorsIfExists().ToAsync();

    /// <summary>
    /// Combina los resultados de <c>ErrorsIfExists</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<T>>> MergeErrorsIfExistsAsync<T>(this Task<IEnumerable<MlResult<T>>> sourceAsync)
        => await (await sourceAsync).MergeErrorsIfExistsAsync();





    /// <summary>
    /// Ejecuta <c>VerifiedEnumerableResultData</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> VerifiedEnumerableResultData<T>(this IEnumerable<MlResult<T>> source)
        => source.Any(x => x.IsFailure) ?
           MergeFailureErrors(source)   :
           MlResult<IEnumerable<T>>.Valid(source.Select(x => x.SuccessValue));




}


/// <summary>
/// Elemento procesado con éxito durante un recorrido particionado: asocia la clave del elemento de origen con el valor obtenido.
/// </summary>
/// <typeparam name="TKey">Tipo de la clave de la entidad.</typeparam>
/// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
/// <param name="Key">Valor de <c>Key</c>.</param>
/// <param name="Value">Valor de <c>Value</c>.</param>
public record TraverseSplitSuccess<TKey, TResult>(TKey Key, TResult Value);

/// <summary>
/// Elemento fallido durante un recorrido particionado: asocia la clave del elemento de origen con los detalles del error.
/// </summary>
/// <typeparam name="TKey">Tipo de la clave de la entidad.</typeparam>
/// <param name="Key">Valor de <c>Key</c>.</param>
/// <param name="Errors">Valor de <c>Errors</c>.</param>
public record TraverseSplitFailure<TKey>(TKey Key, MlErrorDetails Errors);
