// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Métodos de extensión que sustituyen el valor transportado por un <see cref="MlResult{T}"/> por otro valor, manteniendo su condición de válido o fallido.
/// </summary>
public static partial class MlResultChangeReturnResult
{


    #region ChangeReturnResult


    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResult<T, TResult>(this MlResult<T>     source,
                                                                        TResult         validValue,
                                                                        MlErrorDetails failValue)
        => source.Match(
                            valid: x      => validValue.ToMlResultValid(),
                            fail : errors => failValue.ToMlResultFail<TResult>()
                        );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResult<T, TResult>(this MlResult<T> source,
                                                                        TResult     validValue,
                                                                        MlError     failValue)
        => ChangeReturnResult(source, validValue, failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResult<T, TResult>(this MlResult<T>         source,
                                                                        TResult             validValue,
                                                                        IEnumerable<string> failValue)
        => ChangeReturnResult(source, validValue, failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResult<T, TResult>(this MlResult<T> source,
                                                                        TResult     validValue,
                                                                        string      failValue)
        => ChangeReturnResult(source, validValue, failValue.ToMlErrorsDetails());


    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAsync<T, TResult>(this MlResult<T>     source,
                                                                                         TResult         validValue,
                                                                                         MlErrorDetails failValue)
        => await source.MatchAsync(
                                        validAsync: x      => validValue.ToMlResultValidAsync(),
                                        failAsync : errors => failValue.ToMlResultFailAsync<TResult>()
                                    );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                         TResult           validValue,
                                                                                         MlErrorDetails   failValue)
        => (await sourceAsync).Match(
                                        valid: x      => validValue.ToMlResultValid(),
                                        fail : errors => failValue.ToMlResultFail<TResult>()
                                    );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                         TResult           validValue,
                                                                                         MlError           failValue)
        => await ChangeReturnResultAsync<T, TResult>(sourceAsync, validValue, failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                         TResult           validValue,
                                                                                         IEnumerable<string> failValue)
        => await ChangeReturnResultAsync<T, TResult>(sourceAsync, validValue, failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                         TResult           validValue,
                                                                                         string            failValue)
        => await ChangeReturnResultAsync<T, TResult>(sourceAsync, validValue, failValue.ToMlErrorsDetails());




    #endregion ChangeReturnResult


    #region ChangeReturnResultAlwaysValid



    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysValid</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValidValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResultAlwaysValid<T, TResult>(this MlResult<T> source,
                                                                                   TResult     validValue,
                                                                                   TResult     failValidValue)
        => source.Match(
                            valid: x      => validValue    .ToMlResultValid(),
                            fail : errors => failValidValue.ToMlResultValid()
                        );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysValid</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValidValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysValidAsync<T, TResult>(this MlResult<T> source,
                                                                                                    TResult     validValue,
                                                                                                    TResult     failValidValue)
        => await source.MatchAsync(
                                    validAsync: x      => validValue    .ToMlResultValidAsync(),
                                    failAsync : errors => failValidValue.ToMlResultValidAsync()
                                );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysValid</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValidValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysValidAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                                    TResult     validValue,
                                                                                                    TResult     failValidValue)
        => (await sourceAsync).Match(
                                        valid: x      => validValue    .ToMlResultValid(),
                                        fail : errors => failValidValue.ToMlResultValid()
                                    );




    #endregion ChangeReturnResultAlwaysValid


    #region ChangeReturnResultAlwaysFail



    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResultAlwaysFail<T, TResult>(this MlResult<T>     source,
                                                                                  MlErrorDetails validFailValue,
                                                                                  MlErrorDetails failValue)
        => source.Match(
                            valid: x      => validFailValue.ToMlResultFail<TResult>(),
                            fail : errors => failValue     .ToMlResultFail<TResult>()
                        );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResultAlwaysFail<T, TResult>(this MlResult<T> source,
                                                                                  MlError     validFailValue,
                                                                                  MlError     failValue)
        => ChangeReturnResultAlwaysFail<T, TResult>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResultAlwaysFail<T, TResult>(this MlResult<T>         source,
                                                                                  IEnumerable<string> validFailValue,
                                                                                  IEnumerable<string> failValue)
        => ChangeReturnResultAlwaysFail<T, TResult>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ChangeReturnResultAlwaysFail<T, TResult>(this MlResult<T> source,
                                                                                  string      validFailValue,
                                                                                  string      failValue)
        => ChangeReturnResultAlwaysFail<T, TResult>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());




    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysFailAsync<T, TResult>(this MlResult<T>     source,
                                                                                                   MlErrorDetails validFailValue,
                                                                                                   MlErrorDetails failValue)
        => await source.MatchAsync(
                                        validAsync: x      => validFailValue.ToMlResultFailAsync<TResult>(),
                                        failAsync : errors => failValue     .ToMlResultFailAsync<TResult>()
                                  );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysFailAsync<T, TResult>(this MlResult<T>     source,
                                                                                                   MlError     validFailValue,
                                                                                                   MlError     failValue)
        => await ChangeReturnResultAlwaysFailAsync<T, TResult>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysFailAsync<T, TResult>(this MlResult<T>        source,
                                                                                                  IEnumerable<string> validFailValue,
                                                                                                  IEnumerable<string> failValue)
        => await ChangeReturnResultAlwaysFailAsync<T, TResult>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysFailAsync<T, TResult>(this MlResult<T> source,
                                                                                                   string      validFailValue,
                                                                                                   string      failValue)
        => await ChangeReturnResultAlwaysFailAsync<T, TResult>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());



    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysFailAsync<T, TResult>(this Task<MlResult<T>>     sourceAsync,
                                                                                  MlErrorDetails validFailValue,
                                                                                  MlErrorDetails failValue)
        => (await sourceAsync).Match(
                                        valid: x      => validFailValue.ToMlResultFail<TResult>(),
                                        fail : errors => failValue     .ToMlResultFail<TResult>()
                                    );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysFailAsync<T, TResult>(this Task<MlResult<T>>     sourceAsync,
                                                                                  MlError     validFailValue,
                                                                                  MlError     failValue)
        => await ChangeReturnResultAlwaysFailAsync<T, TResult>(sourceAsync, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysFailAsync<T, TResult>(this Task<MlResult<T>>     sourceAsync,
                                                                                  IEnumerable<string> validFailValue,
                                                                                  IEnumerable<string> failValue)
        => await ChangeReturnResultAlwaysFailAsync<T, TResult>(sourceAsync, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaysFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ChangeReturnResultAlwaysFailAsync<T, TResult>(this Task<MlResult<T>>     sourceAsync,
                                                                                  string      validFailValue,
                                                                                  string      failValue)
        => await ChangeReturnResultAlwaysFailAsync<T, TResult>(sourceAsync, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());




    #endregion ChangeReturnResultAlwaysFail


    #region ChangeReturnResultIfValid



    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c> unicamente cuando el resultado de origen es válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnValidValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ChangeReturnResultIfValid<T>(this MlResult<T> source,
                                                                T           returnValidValue)
        => source.Match(
                            valid: x      => returnValidValue.ToMlResultValid(),
                            fail : errors => errors.ToMlResultFail<T>()
                        );
    

    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c> unicamente cuando el resultado de origen es válido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnValidValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidAsync<T>(this MlResult<T> source,
                                                                                 T           returnValidValue)
        => await source.MatchAsync(
                                        validAsync: x      => returnValidValue.ToMlResultValidAsync(),
                                        failAsync : errors => errors          .ToMlResultFailAsync<T>()
                                  );


    /// <summary>
    /// Ejecuta <c>ChangeReturnResult</c> unicamente cuando el resultado de origen es válido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="returnValidValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                 T                 returnValidValue)
        => (await sourceAsync).Match(
                                        valid: x      => returnValidValue.ToMlResultValid(),
                                        fail : errors => errors          .ToMlResultFail<T>()
                                    );


    #endregion ChangeReturnResultIfValid


    #region ChangeReturnResultIfValidToFail



    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ChangeReturnResultIfValidToFail<T>(this MlResult<T>     source,
                                                                      MlErrorDetails returnFailValue)
        => source.Match(
                            valid: x      => returnFailValue.ToMlResultFail<T>(),
                            fail : errors => errors.ToMlResultFail<T>()
                        );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ChangeReturnResultIfValidToFail<T>(this MlResult<T> source,
                                                                      MlError     returnFailValue)
        => ChangeReturnResultIfValidToFail(source, returnFailValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnFailValues">Colección de <c>returnFailValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ChangeReturnResultIfValidToFail<T>(this MlResult<T>         source,
                                                                      IEnumerable<string> returnFailValues)
        => ChangeReturnResultIfValidToFail(source, returnFailValues.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ChangeReturnResultIfValidToFail<T>(this MlResult<T> source,
                                                                      string      returnFailValue)
        => ChangeReturnResultIfValidToFail(source, returnFailValue.ToMlErrorsDetails());



    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this MlResult<T>     source,
                                                                                       MlErrorDetails returnFailValue)
        => await source.MatchAsync(
                            validAsync: x      => returnFailValue.ToMlResultFailAsync<T>(),
                            failAsync : errors => errors         .ToMlResultFailAsync<T>()
                        );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this MlResult<T>     source,
                                                                      MlError     returnFailValue)
        => await ChangeReturnResultIfValidToFailAsync(source, returnFailValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnFailValues">Colección de <c>returnFailValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this MlResult<T>     source,
                                                                      IEnumerable<string> returnFailValues)
        => await ChangeReturnResultIfValidToFailAsync(source, returnFailValues.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this MlResult<T>     source,
                                                                      string      returnFailValue)
        => await ChangeReturnResultIfValidToFailAsync(source, returnFailValue.ToMlErrorsDetails());



    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       MlErrorDetails   returnFailValue)
        => (await sourceAsync).Match(
                                        valid: x      => returnFailValue.ToMlResultFail<T>(),
                                        fail : errors => errors.ToMlResultFail<T>()
                                    );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       MlError           returnFailValue)
        => await ChangeReturnResultIfValidToFailAsync(sourceAsync, returnFailValue.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="returnFailValues">Colección de <c>returnFailValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this Task<MlResult<T>>   sourceAsync,
                                                                                       IEnumerable<string> returnFailValues)
        => await ChangeReturnResultIfValidToFailAsync(sourceAsync, returnFailValues.ToMlErrorsDetails());

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfValidToFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="returnFailValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       string            returnFailValue)
        => await ChangeReturnResultIfValidToFailAsync(sourceAsync, returnFailValue.ToMlErrorsDetails());





    #endregion ChangeReturnResultIfValidToFail



    #region ChangeReturnResultIfFailToValid



    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfFailToValid</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnValidValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ChangeReturnResultIfFailToValid<T>(this MlResult<T> source,
                                                                      T           returnValidValue)
        => source.Match(
                            valid: x      => source,
                            fail : errors => returnValidValue.ToMlResultValid()
                        );


    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfFailToValid</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="returnValidValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfFailToValidAsync<T>(this MlResult<T> source,
                                                                                       T           returnValidValue)
        => await source.MatchAsync(
                            valid     : x      => source,
                            failAsync : errors => returnValidValue.ToMlResultValidAsync()
                        );

    /// <summary>
    /// Ejecuta <c>ChangeReturnResultIfFailToValid</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="returnValidValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ChangeReturnResultIfFailToValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       T                 returnValidValue)
        => (await sourceAsync).ChangeReturnResultIfFailToValid(returnValidValue);




    #endregion ChangeReturnResultIfFailToValid



    #region Sin_Usar


    #endregion Sin_Usar


    ////public async static Task<MlResult<TResult>> ChangeReturnResultFaillureAsync<T, TResult>(this Task<MlResult<TResult>> sourceAsync,
    ////                                                                                     TResult         returnValidValue)
    ////{
    ////    var result = (await sourceAsync).Match(
    ////                                    valid: x      => sourceAsync,
    ////                                    fail: errors => returnValidValue.ToMlResultValid()
    ////                                );
    ////    return result;
    ////}


    ////public static MlResult<TResult> ChangeReturnFailure<T, TResult>(this MlResult<TResult> source,
    ////                                                                   IEnumerable<string> failValue)
    ////    => source.Match<T, TResult>(
    ////                                    valid: x => successFunc(x).ToMlResultValid(),
    ////                                    fail: errors => failValue.ToMlResultFail<TResult>()
    ////                                );


}

