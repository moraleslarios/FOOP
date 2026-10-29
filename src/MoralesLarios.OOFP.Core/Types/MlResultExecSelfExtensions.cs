// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Métodos de extensión que ejecutan acciones laterales sobre un <see cref="MlResult{T}"/> (registro, trazas, notificaciones) y devuelven el resultado de origen sin modificarlo.
/// </summary>
public static class MlResultExecSelfExtensions
{


    #region ExecSelf





    /// <summary>
    /// Execute the actionSuccess if the source is valid, otherwise execute the actionFailure. Return same source every time.
    /// </summary>
    /// <typeparam name="T">Value type of the source result.</typeparam>
    /// <param name="source">Source result.</param>
    /// <param name="actionValid">Action executed when the source is valid.</param>
    /// <param name="actionFail">Action executed when the source is a failure.</param>
    /// <returns>The same <paramref name="source"/> result, unchanged.</returns>
    public static MlResult<T> ExecSelf<T>(this MlResult<T>             source, 
                                               Action<T>               actionValid,
                                               Action<MlErrorDetails> actionFail)
        => source.Match(
                            valid: x      => { actionValid(x)    ; return source; },
                            fail : errors => { actionFail(errors); return source; }
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this MlResult<T>                 source,
                                                                Func<T              , Task> actionValidAsync,
                                                                Func<MlErrorDetails, Task> actionFailAsync)
        => await source.MatchAsync(
                                        validAsync: async x      => { await actionValidAsync(x)    ; return source; },
                                        failAsync : async errors => { await actionFailAsync(errors); return source; }
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                Func<T              , Task> actionValidAsync,
                                                                Func<MlErrorDetails, Task> actionFailAsync)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => { await actionValidAsync(x    ); return await sourceAsync; },
                                            failAsync : async errors => { await actionFailAsync(errors); return await sourceAsync; }
                                       );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                Action<T>                   actionValid,
                                                                Func<MlErrorDetails, Task> actionFailAsync)
        => await (await sourceAsync).ExecSelfAsync(actionValid.ToFuncTask(), actionFailAsync);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                Func<T              , Task> actionValidAsync,
                                                                Action<MlErrorDetails>     actionFail)
        => await (await sourceAsync).ExecSelfAsync(actionValidAsync, actionFail.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                Action<T>               actionValid,
                                                                Action<MlErrorDetails> actionFail)
        => await (await sourceAsync).ExecSelfAsync(actionValid.ToFuncTask(), actionFail.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this MlResult<T>                 source,
                                                                Action<T>                   actionValid,
                                                                Func<MlErrorDetails, Task> actionFailAsync)
        => await source.ExecSelfAsync(actionValid.ToFuncTask(), actionFailAsync);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this MlResult<T>                 source,
                                                                Func<T              , Task> actionValidAsync,
                                                                Action<MlErrorDetails>     actionFail)
        => await source.ExecSelfAsync(actionValidAsync, actionFail.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ExecSelfAsync<T>(this MlResult<T>             source,
                                                         Action<T>               actionValid,
                                                         Action<MlErrorDetails> actionFail)
        => source.ExecSelf(actionValid, actionFail).ToAsync();



    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ExecSelf<T>(this MlResult<T> source, 
                                               Action      action)
        => source.Match(
                            valid: _ => { action(); return source; },
                            fail : _ => { action(); return source; }
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this MlResult<T> source,
                                                                Func<Task> actionAsync)
        => await source.MatchAsync(
                                        validAsync: async _ => { await actionAsync(); return source; },
                                        failAsync : async _ => { await actionAsync(); return source; }
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                Func<Task>        actionAsync)
        => await sourceAsync.MatchAsync(
                                            validAsync: async _ => { await actionAsync(); return await sourceAsync; },
                                            failAsync : async _ => { await actionAsync(); return await sourceAsync; }
                                       );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                Action            action)
        => await (await sourceAsync).ExecSelfAsync(action.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ExecSelfAsync<T>(this MlResult<T> source,
                                                        Action            action)
        => source.ExecSelf(action).ToAsync();





    /// <summary>
    /// Execute the actionSuccess if the source is valid, otherwise execute the actionFailure. Return same source every time.
    /// If an exception is thrown, the errorMessageBuilder is used to build the error message.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelf<T>(this MlResult<T>             source,
                                                  Action<T>               actionValid,
                                                  Action<MlErrorDetails> actionFail,
                                                  Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => actionValid.TryToMlResult(x, errorMessageBuilder),
                            fail : errors => MlResultConversions.TryToMlResultErrors<T>(actionFail, errors, errorMessageBuilder)
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelf<T>(this MlResult<T>             source,
                                                  Action<T>               actionValid,
                                                  Action<MlErrorDetails> actionFail,
                                                  string                  errorMessage = null!)
        => source.TryExecSelf(actionValid, actionFail, _ => errorMessage!);



    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>                 source,
                                                                   Func<T, Task>               actionValidAsync,
                                                                   Func<MlErrorDetails, Task> actionFailAsync,
                                                                   Func<Exception, string>     errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: async x      => await actionValidAsync.TryToMlResultAsync         (x     , errorMessageBuilder),
                                        failAsync : async errors => await actionFailAsync .TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>                 source,
                                                                   Func<T, Task>               actionValidAsync,
                                                                   Func<MlErrorDetails, Task> actionFailAsync,
                                                                   string                      errorMessage = null!)
        => await source.TryExecSelfAsync(actionValidAsync, actionFailAsync, _ => errorMessage!);



    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                   Func<T, Task>               actionValidAsync,
                                                                   Func<MlErrorDetails, Task> actionFailAsync,
                                                                   Func<Exception, string>     errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => await actionValidAsync.TryToMlResultAsync        (x     , errorMessageBuilder),
                                            failAsync : async errors => await actionFailAsync.TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                   Func<T, Task>               actionValidAsync,
                                                                   Func<MlErrorDetails, Task> actionFailAsync,
                                                                   string                      errorMessage = null!)
        => await sourceAsync.TryExecSelfAsync(actionValidAsync, actionFailAsync, _ => errorMessage!);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                   Action<T>                   actionValid,
                                                                   Func<MlErrorDetails, Task> actionFailAsync,
                                                                   Func<Exception, string>     errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfAsync(actionValid.ToFuncTask(), actionFailAsync, errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                   Action<T>                   actionValid,
                                                                   Func<MlErrorDetails, Task> actionFailAsync,
                                                                   string                      errorMessage = null!)
        => await (await sourceAsync).TryExecSelfAsync(actionValid.ToFuncTask(), actionFailAsync, errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Func<T, Task>           actionValidAsync,
                                                                   Action<MlErrorDetails> actionFail,
                                                                   Func<Exception, string> errorMessageBuilder)
        => await source.TryExecSelfAsync(actionValidAsync, actionFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Func<T, Task>           actionValidAsync,
                                                                   Action<MlErrorDetails> actionFail,
                                                                   string                  errorMessage = null!)
        => await source.TryExecSelfAsync(actionValidAsync, actionFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Action<T>               actionValid,
                                                                   Action<MlErrorDetails> actionFail,
                                                                   Func<Exception, string> errorMessageBuilder)
        => await source.TryExecSelfAsync(actionValid.ToFuncTask(), actionFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Action<T>               actionValid,
                                                                   Action<MlErrorDetails> actionFail,
                                                                   string                  errorMessage = null!)
        => await source.TryExecSelfAsync(actionValid.ToFuncTask(), actionFail.ToFuncTask(), errorMessage);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelf<T>(this MlResult<T>             source,
                                                  Action                  action,
                                                  Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => action.TryToMlResult(x, errorMessageBuilder),
                            fail : errors => MlResultConversions.TryToMlResultErrors<T>(action, errors, errorMessageBuilder)
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelf<T>(this MlResult<T>             source,
                                                  Action                  action,
                                                  string                  errorMessage = null!)
        => source.TryExecSelf(action, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                   Action                  action,
                                                                   Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryExecSelf(action, errorMessageBuilder);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                   Action            action,
                                                                   string            errorMessage = null!)
        => await sourceAsync.TryExecSelfAsync(action, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Func<Task>              actionAsync,
                                                                   Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: async x      => await actionAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                        failAsync : async errors => await actionAsync.TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T> source,
                                                                   Func<Task>  actionAsync,
                                                                   string      errorMessage = null!)
        => await source.TryExecSelfAsync(actionAsync, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                            Action                  action,
                                                            Func<Exception, string> errorMessageBuilder)
        => source.TryExecSelf(action, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                            Action                  action,
                                                            string                  errorMessage = null!)
        => source.TryExecSelf(action, errorMessage).ToAsync();

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                   Func<Task>              actionAsync,
                                                                   Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfAsync(actionAsync, errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                   Func<Task>  actionAsync,
                                                                   string      errorMessage = null!)
        => await (await sourceAsync).TryExecSelfAsync(actionAsync, errorMessage);





    #endregion


    #region ExecSelfIf


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrue">Acción a ejecutar.</param>
    /// <param name="actionFalse">Acción a ejecutar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ExecSelfIf<T>(this MlResult<T>   source,
                                                 Func<T, bool> condition,
                                                 Action<T>     actionTrue,
                                                 Action<T>     actionFalse)
        => source.Match(
                            valid: x      => 
                                            { 
                                                if (condition(x)) actionTrue(x); else actionFalse(x);
                                                
                                                return source; 
                                            },
                            fail : MlResult<T>.Fail
                        );


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrueAsync">Función a aplicar.</param>
    /// <param name="actionFalseAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> ExecSelfIfAsync<T>(this MlResult<T>   source,
                                                                  Func<T, bool> condition,
                                                                  Func<T, Task> actionTrueAsync,
                                                                  Func<T, Task> actionFalseAsync)
        => await source.MatchAsync(
                                        validAsync: async x =>
                                                            {
                                                                if (condition(x)) await actionTrueAsync(x); else await actionFalseAsync(x);

                                                                return source;
                                                            },
                                        fail : MlResult<T>.Fail
                                   );


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrueAsync">Función a aplicar.</param>
    /// <param name="actionFalseAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> ExecSelfIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                  Func<T, bool>     condition,
                                                                  Func<T, Task>     actionTrueAsync,
                                                                  Func<T, Task>     actionFalseAsync)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      =>
                                            {
                                                if (condition(x)) await actionTrueAsync(x); else await actionFalseAsync(x);
                                                return await sourceAsync;
                                            },
                                            fail: MlResult<T>.Fail
                                       );


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrue">Acción a ejecutar.</param>
    /// <param name="actionFalseAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> ExecSelfIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                  Func<T, bool>     condition,
                                                                  Action<T>         actionTrue,
                                                                  Func<T, Task>     actionFalseAsync)
        => await (await sourceAsync).ExecSelfIfAsync(condition, actionTrue.ToFuncTask(), actionFalseAsync);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrueAsync">Función a aplicar.</param>
    /// <param name="actionFalse">Acción a ejecutar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> ExecSelfIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                  Func<T, bool>     condition,
                                                                  Func<T, Task>     actionTrueAsync,
                                                                  Action<T>         actionFalse)
        => await (await sourceAsync).ExecSelfIfAsync(condition, actionTrueAsync, actionFalse.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrue">Acción a ejecutar.</param>
    /// <param name="actionFalse">Acción a ejecutar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> ExecSelfIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                  Func<T, bool>     condition,
                                                                  Action<T>         actionTrue,
                                                                  Action<T>         actionFalse)
        => await (await sourceAsync).ExecSelfIfAsync(condition, actionTrue.ToFuncTask(), actionFalse.ToFuncTask());





    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrue">Acción a ejecutar.</param>
    /// <param name="actionFalse">Acción a ejecutar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfIf<T>(this MlResult<T>             source,
                                                    Func<T, bool>           condition,
                                                    Action<T>               actionTrue,
                                                    Action<T>               actionFalse,
                                                    Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x => condition(x) ? actionTrue.TryToMlResult(x, errorMessageBuilder) : actionFalse.TryToMlResult(x, errorMessageBuilder),
                            fail :      MlResult<T>.Fail
                        );


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrue">Acción a ejecutar.</param>
    /// <param name="actionFalse">Acción a ejecutar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfIf<T>(this MlResult<T>   source,
                                                    Func<T, bool> condition,
                                                    Action<T>     actionTrue,
                                                    Action<T>     actionFalse,
                                                    string        errorMessage = null!)
        => source.TryExecSelfIf(condition, actionTrue, actionFalse, _ => errorMessage!);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrueAsync">Función a aplicar.</param>
    /// <param name="actionFalse">Acción a ejecutar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> TryExecSelfIfAsync<T>(this MlResult<T>             source,
                                                                     Func<T, bool>           condition,
                                                                     Func<T, Task>           actionTrueAsync,
                                                                     Action<T>               actionFalse,
                                                                     Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: async x => condition(x) ? await actionTrueAsync.TryToMlResultAsync(x, errorMessageBuilder) : actionFalse.TryToMlResult(x, errorMessageBuilder),
                                        fail      :            MlResult<T>.Fail
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrueAsync">Función a aplicar.</param>
    /// <param name="actionFalse">Acción a ejecutar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> TryExecSelfIfAsync<T>(this MlResult<T>   source,
                                                                     Func<T, bool> condition,
                                                                     Func<T, Task> actionTrueAsync,
                                                                     Action<T>     actionFalse,
                                                                     string        errorMessage = null!)
        => await source.TryExecSelfIfAsync(condition, actionTrueAsync, actionFalse, _ => errorMessage!);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrueAsync">Función a aplicar.</param>
    /// <param name="actionFalseAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> TryExecSelfIfAsync<T>(this MlResult<T>             source,
                                                                     Func<T, bool>           condition,
                                                                     Func<T, Task>           actionTrueAsync,
                                                                     Func<T, Task>           actionFalseAsync,
                                                                     Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: async x => condition(x) ? await actionTrueAsync.TryToMlResultAsync(x, errorMessageBuilder) : await actionFalseAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                        fail      :            MlResult<T>.Fail
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrueAsync">Función a aplicar.</param>
    /// <param name="actionFalseAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> TryExecSelfIfAsync<T>(this MlResult<T>   source,
                                                                     Func<T, bool> condition,
                                                                     Func<T, Task> actionTrueAsync,
                                                                     Func<T, Task> actionFalseAsync,
                                                                     string        errorMessage = null!)
        => await source.TryExecSelfIfAsync(condition, actionTrueAsync, actionFalseAsync, _ => errorMessage!);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrue">Acción a ejecutar.</param>
    /// <param name="actionFalseAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> TryExecSelfIfAsync<T>(this MlResult<T>             source,
                                                                     Func<T, bool>           condition,
                                                                     Action<T>               actionTrue,
                                                                     Func<T, Task>           actionFalseAsync,
                                                                     Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: async x => condition(x) ? actionTrue.TryToMlResult(x, errorMessageBuilder) : await actionFalseAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                        fail      :            MlResult<T>.Fail
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>If</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="actionTrue">Acción a ejecutar.</param>
    /// <param name="actionFalseAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> TryExecSelfIfAsync<T>(this MlResult<T>   source,
                                                                     Func<T, bool> condition,
                                                                     Action<T>     actionTrue,
                                                                     Func<T, Task> actionFalseAsync,
                                                                     string        errorMessage = null!)
        => await source.TryExecSelfIfAsync(condition, actionTrue, actionFalseAsync, _ => errorMessage!);



    #endregion




    #region ExecSelfIfValid



    /// <summary>
    /// Execute the actionSuccess if the source is valid, otherwise execute the actionFailure. Return same source every time.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ExecSelfIfValid<T>(this MlResult<T> source,
                                                      Action<T>   actionValid)
        => source.Match(
                            valid: x      => { actionValid(x); return source; },
                            fail : errors => source
                        );
    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfValidAsync<T>(this MlResult<T>   source,
                                                                       Func<T, Task> actionValidAsync)
        => await source.MatchAsync(
                                        validAsync: async x      => { await actionValidAsync(x); return source; },
                                        failAsync :       errors => source.ToAsync()
                                   );
    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                       Func<T, Task>     actionValidAsync)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => { await actionValidAsync(x); return await sourceAsync; },
                                            failAsync : async errors => await sourceAsync
                                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                       Action<T>         actionValid)
        => await (await sourceAsync).ExecSelfIfValidAsync(actionValid.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ExecSelfIfValidAsync<T>(this MlResult<T> source,
                                                                Action<T>         actionValid)
        => source.ExecSelfIfValid(actionValid).ToAsync();


    /// <summary>
    /// Execute the actionSuccess if the source is valid, otherwise execute the actionFailure. Return same source every time.
    /// If an exception is thrown, the errorMessageBuilder is used to build the error message.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfIfValid<T>(this MlResult<T>             source,
                                                         Action<T>               actionValid,
                                                         Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => actionValid.TryToMlResult(x, errorMessageBuilder),
                            fail : errors => source
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfIfValid<T>(this MlResult<T> source,
                                                         Action<T>   actionValid,
                                                         string      errorMessage = null!)
        => source.TryExecSelfIfValid(actionValid, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this MlResult<T>             source,
                                                                          Func<T, Task>   actionValidAsync,
                                                                          Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: async x => await actionValidAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                        failAsync : errors => source.ToAsync()
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this MlResult<T>   source,
                                                                          Func<T, Task> actionValidAsync,
                                                                          string        errorMessage = null!)
        => await source.TryExecSelfIfValidAsync(actionValidAsync, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                          Func<T, Task>           actionValidAsync,
                                                                          Func<Exception, string> errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => await actionValidAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                            failAsync : async errors => await sourceAsync
                                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValidAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          Func<T, Task>     actionValidAsync,
                                                                          string            errorMessage = null!)
        => await sourceAsync.TryExecSelfIfValidAsync(actionValidAsync, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                          Action<T>               actionValid,
                                                                          Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfValidAsync(actionValid.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          Action<T>         actionValid,
                                                                          string            errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfValidAsync(actionValid.ToFuncTask(), errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this MlResult<T>             source,
                                                                   Action<T>               actionValid,
                                                                   Func<Exception, string> errorMessageBuilder)
        => source.TryExecSelfIfValid(actionValid, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es válido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionValid">Acción que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this MlResult<T> source,
                                                                   Action<T>   actionValid,
                                                                   string      errorMessage = null!)
        => source.TryExecSelfIfValid(actionValid, errorMessage).ToAsync();


    #endregion


    #region ExecSelfIfFail







    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ExecSelfIfFail<T>(this MlResult<T>             source,
                                                     Action<MlErrorDetails> actionFail)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => { actionFail(errors); return source; }
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailAsync<T>(this MlResult<T>                 source,
                                                                      Func<MlErrorDetails, Task> actionFailAsync)
        => await source.MatchAsync(
                                        validAsync:      x      => x.ToMlResultValid().ToAsync(),
                                        failAsync: async errors => { await actionFailAsync(errors); return source; }
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                    Func<MlErrorDetails, Task> actionFailAsync)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => await sourceAsync,
                                            failAsync : async errors => { await actionFailAsync(errors); return await sourceAsync; }
                                       );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                    Action<MlErrorDetails> actionFail)
        => (await sourceAsync).ExecSelfIfFail(actionFail);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ExecSelfIfFailAsync<T>(this MlResult<T>             source,
                                                               Action<MlErrorDetails> actionFail)
        => source.ExecSelfIfFail(actionFail).ToAsync();

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                     Func<MlErrorDetails, Task> actionFailAsync)
        => await sourceAsync.ExecSelfFailAsync(actionFailAsync);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                     Action<MlErrorDetails> actionFail)
        => await sourceAsync.ExecSelfFailAsync(actionFail);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>Fail</c>. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfFail<T>(this MlResult<T>             source,
                                                      Action<MlErrorDetails> actionFail,
                                                      Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => MlResultConversions.TryToMlResultErrors<T>(actionFail, errors, errorMessageBuilder)
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>Fail</c>. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfFail<T>(this MlResult<T>             source,
                                                      Action<MlErrorDetails> actionFail,
                                                      string                  errorMessage = null!)
        => source.TryExecSelfFail(actionFail, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this MlResult<T>                 source,
                                                                         Func<MlErrorDetails, Task> actionFailAsync,
                                                                         Func<Exception, string>     errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: x => x.ToMlResultValid().ToAsync(),
                                        failAsync : async errors => await actionFailAsync.TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                   );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this MlResult<T>                 source,
                                                                         Func<MlErrorDetails, Task> actionFailAsync,
                                                                         string                      errorMessage = null!)
        => await source.TryExecSelfIfFailAsync(actionFailAsync, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>Fail</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                       Func<MlErrorDetails, Task> actionFailAsync,
                                                                       Func<Exception, string>     errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                                            validAsync:       x      => x.ToMlResultValidAsync(),
                                            failAsync : async errors => await actionFailAsync.TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>Fail</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                       Func<MlErrorDetails, Task> actionFailAsync,
                                                                       string                      errorMessage = null!)
        => await sourceAsync.TryExecSelfFailAsync(actionFailAsync, _ => errorMessage!);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>Fail</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                       Action<MlErrorDetails> actionFail,
                                                                       Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailAsync(actionFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo de
    /// <c>Fail</c>. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                       Action<MlErrorDetails> actionFail,
                                                                       string                  errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailAsync(actionFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this MlResult<T>             source,
                                                                  Action<MlErrorDetails> actionFail,
                                                                  Func<Exception, string> errorMessageBuilder)
        => source.TryExecSelfFail(actionFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this MlResult<T>             source,
                                                                  Action<MlErrorDetails> actionFail,
                                                                  string                  errorMessage = null!)
        => source.TryExecSelfFail(actionFail, errorMessage).ToAsync();

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                        Func<MlErrorDetails, Task> actionFailAsync,
                                                                        Func<Exception, string>     errorMessageBuilder)
        => await sourceAsync.TryExecSelfFailAsync(actionFailAsync, errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailAsync">Acción asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                        Func<MlErrorDetails, Task> actionFailAsync,
                                                                        string                      errorMessage = null!)
        => await sourceAsync.TryExecSelfFailAsync(actionFailAsync, errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                        Action<MlErrorDetails> actionFail,
                                                                        Func<Exception, string> errorMessageBuilder)
        => await sourceAsync.TryExecSelfFailAsync(actionFail, errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                        Action<MlErrorDetails> actionFail,
                                                                        string                  errorMessage = null!)
        => await sourceAsync.TryExecSelfFailAsync(actionFail, errorMessage);


    #endregion


    #region ExecSelfIfFailWithValue





    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ExecSelfIfFailWithValue<T, TValue>(this MlResult<T>    source,
                                                                      Action<TValue> actionFailValue)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => errors.GetMetadataValue<TValue>()
                                                        .Bind(value =>
                                                        {
                                                            actionFailValue(value);

                                                            return source;
                                                        })
                        );


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValueAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>        source,
                                                                                       Func<TValue, Task> actionFailValueAsync)
        => await source.MatchAsync(
                                        validAsync: x      => x.ToMlResultValidAsync(),
                                        failAsync : errors => errors.GetMetadataValue<TValue>()
                                                                    .BindAsync(async value =>
                                                                    {
                                                                        await actionFailValueAsync(value);

                                                                        return source;
                                                                    })
                                    );


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailValueAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>  sourceAsync,
                                                                                       Func<TValue, Task> actionFailValueAsync)
        => await (await sourceAsync).ExecSelfIfFailWithValueAsync(actionFailValueAsync);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>> sourceAsync,
                                                                                       Action<TValue>    actionFailValue)
        => await (await sourceAsync).ExecSelfIfFailWithValueAsync(actionFailValue.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>    source,
                                                                                 Action<TValue> actionFailValue)
        => source.ExecSelfIfFailWithValue<T, TValue>(actionFailValue).ToAsync();



    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfIfFailWithValue<T, TValue>(this MlResult<T>             source,
                                                                         Action<TValue>          actionFailValue,
                                                                         Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => errors.GetMetadataValue<TValue>()
                                                        .Bind(value =>
                                                        {
                                                            var actionResult = actionFailValue.TryToMlResult(value, errorMessageBuilder);

                                                            var result = actionResult.Match(
                                                                                                fail : errorDetails => source.FailureDetails.Merge(errorDetails).ToMlResultFail<T>(),
                                                                                                valid: _            => source
                                                                                            );

                                                            return result;
                                                        })
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfIfFailWithValue<T, TValue>(this MlResult<T>    source,
                                                                         Action<TValue> actionFailValue,
                                                                         string         errorMessage = null!)
        => source.TryExecSelfIfFailWithValue<T, TValue>(actionFailValue, _ => errorMessage!);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValueAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>             source,
                                                                                          Func<TValue, Task>      actionFailValueAsync,
                                                                                          Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                            validAsync: x      => x.ToMlResultValidAsync(),
                            failAsync : errors => errors.GetMetadataValue<TValue>()
                                                        .BindAsync(async value =>
                                                        {
                                                            var actionResult = await actionFailValueAsync.TryToMlResultAsync(value, errorMessageBuilder);

                                                            var result = actionResult.Match(
                                                                                                fail : errorDetails => source.FailureDetails.Merge(errorDetails).ToMlResultFail<T>(),
                                                                                                valid: _            => source
                                                                                            );

                                                            return result;
                                                        })
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValueAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>        source,
                                                                                          Func<TValue, Task> actionFailValueAsync,
                                                                                          string             errorMessage = null!)
        => await source.TryExecSelfIfFailWithValueAsync(actionFailValueAsync, _ => errorMessage!);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailValueAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>       sourceAsync,
                                                                                          Func<TValue, Task>      actionFailValueAsync,
                                                                                          Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailWithValueAsync(actionFailValueAsync, errorMessageBuilder);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailValueAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>  sourceAsync,
                                                                                          Func<TValue, Task> actionFailValueAsync,
                                                                                          string             errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailWithValueAsync(actionFailValueAsync, errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>       sourceAsync,
                                                                                          Action<TValue>          actionFailValue,
                                                                                          Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailWithValueAsync(actionFailValue.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>> sourceAsync,
                                                                                          Action<TValue>    actionFailValue,
                                                                                          string            errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailWithValueAsync(actionFailValue.ToFuncTask(), errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>             source,
                                                                                    Action<TValue>          actionFailValue,
                                                                                    Func<Exception, string> errorMessageBuilder)
        => source.TryExecSelfIfFailWithValue<T, TValue>(actionFailValue, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailValue">Valor sobre el que se opera.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>    source,
                                                                                    Action<TValue> actionFailValue,
                                                                                    string         errorMessage = null!)
        => source.TryExecSelfIfFailWithValue<T, TValue>(actionFailValue, errorMessage).ToAsync();


    #endregion


    #region ExecSelfIfFailWithException





    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ExecSelfIfFailWithException<T>(this MlResult<T>       source,
                                                                  Action<Exception> actionFailException)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => errors.GetException()
                                                                .Bind(exception =>
                                                                {
                                                                    actionFailException(exception);

                                                                    return source;
                                                                })
                        );


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailExceptionAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>           source,
                                                                                   Func<Exception, Task> actionFailExceptionAsync)
        => await source.MatchAsync(
                                        validAsync: x      => x.ToMlResultValidAsync(),
                                        failAsync : errors => errors.GetExceptionAsync()
                                                                            .BindAsync(async exception =>
                                                                            {
                                                                                await actionFailExceptionAsync(exception);

                                                                                return source;
                                                                            })
                                    );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailExceptionAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>>     sourceAsync,
                                                                                   Func<Exception, Task> actionFailExceptionAsync)
        => await (await sourceAsync).ExecSelfIfFailWithExceptionAsync(actionFailExceptionAsync);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                   Action<Exception> actionFailException)
        => await (await sourceAsync).ExecSelfIfFailWithExceptionAsync(actionFailException.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>       source,
                                                                            Action<Exception> actionFailException)
        => source.ExecSelfIfFailWithException(actionFailException).ToAsync();



    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T>             source,
                                                                     Action<Exception>       actionFailException,
                                                                     Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x => x.ToMlResultValid(),
                            fail: errors => errors.GetException()
                                                        .Bind(exception =>
                                                        {
                                                            var actionResult = actionFailException.TryToMlResult(exception, errorMessageBuilder);

                                                            var result = actionResult.Match(
                                                                                                fail: errorDetails => source.FailureDetails.Merge(errorDetails).ToMlResultFail<T>(),
                                                                                                valid: _ => source
                                                                                            );

                                                            return result;
                                                        })
                        );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T>       source,
                                                                     Action<Exception> actionFailException,
                                                                     string            errorMessage = null!)
        => source.TryExecSelfIfFailWithException<T>(actionFailException, _ => errorMessage!);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailExceptionAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>             source,
                                                                                      Func<Exception, Task>   actionFailExceptionAsync,
                                                                                      Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: x => x.ToMlResultValidAsync(),
                                        failAsync : errors => errors.GetExceptionAsync()
                                                                    .BindAsync(async exception =>
                                                                    {
                                                                        var actionResult = await actionFailExceptionAsync.TryToMlResultAsync(exception, errorMessageBuilder);

                                                                        var result = await actionResult.MatchAsync(
                                                                                                            failAsync : async errorDetails => await source.FailureDetails.Merge(errorDetails).ToMlResultFailAsync<T>(),
                                                                                                            validAsync: async _            => await source.ToAsync()
                                                                                                        );

                                                                        return result;
                                                                    })
                                    );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailExceptionAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>            source,
                                                                                      Func<Exception, Task>  actionFailExceptionAsync,
                                                                                      string                 errorMessage = null!)
        => await source.TryExecSelfIfFailWithExceptionAsync(actionFailExceptionAsync, _ => errorMessage!);


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailExceptionAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                                      Func<Exception, Task>   actionFailExceptionAsync,
                                                                                      Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailWithExceptionAsync(actionFailExceptionAsync, errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailExceptionAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>>      sourceAsync,
                                                                                      Func<Exception, Task>  actionFailExceptionAsync,
                                                                                      string                 errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailWithExceptionAsync(actionFailExceptionAsync, errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                                      Action<Exception>       actionFailException,
                                                                                      Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailWithExceptionAsync(actionFailException.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                      Action<Exception> actionFailException,
                                                                                      string            errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailWithExceptionAsync(actionFailException.ToFuncTask(), errorMessage);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>             source,
                                                                               Action<Exception>       actionFailException,
                                                                               Func<Exception, string> errorMessageBuilder)
        => source.TryExecSelfIfFailWithException<T>(actionFailException, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error contiene una excepción. Captura
    /// cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailException">Excepción capturada.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>       source,
                                                                               Action<Exception> actionFailException,
                                                                               string            errorMessage = null!)
        => source.TryExecSelfIfFailWithException<T>(actionFailException, errorMessage).ToAsync();

    #endregion


    #region ExecSelfIfFailWithoutException


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ExecSelfIfFailWithoutException<T>(this MlResult<T>             source,
                                                                     Action<MlErrorDetails> actionFail)
        => source.Match(
                            valid: x             => x.ToMlResultValid(),
                            fail : errorsDetails => errorsDetails.GetException()
                                                                .BindIfFail( errorsResult =>
                                                                {
                                                                    actionFail(errorsDetails);

                                                                    return errorsResult;
                                                                })
                                                                .Bind( _ => source)
                        );


    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error NO contiene una excepción. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFailExceptionAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(this MlResult<T>                 source,
                                                                                      Func<MlErrorDetails, Task> actionFailExceptionAsync)
        => await source.MatchAsync(
                                        validAsync: x             => x.ToMlResultValidAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync()
                                                                                    .BindIfFailAsync(async errorsResult =>
                                                                                    {
                                                                                        await actionFailExceptionAsync(errorsDetails);

                                                                                        return errorsResult;
                                                                                    })
                                                                                    .BindAsync( _ => source.ToAsync())
                                    );

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error NO contiene una excepción. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFailExceptionAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                                      Func<MlErrorDetails, Task> actionFailExceptionAsync)
        => await (await sourceAsync).ExecSelfIfFailWithoutExceptionAsync(actionFailExceptionAsync);

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error NO contiene una excepción. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                                      Action<MlErrorDetails> actionFail)
        => await (await sourceAsync).ExecSelfIfFailWithoutExceptionAsync(actionFail.ToFuncTask());

    /// <summary>
    /// Ejecuta una acción como efecto lateral y devuelve el resultado de origen sin modificarlo unicamente
    /// cuando el resultado de origen es fallido y su detalle de error NO contiene una excepción. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionFail">Acción que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(this MlResult<T>             source,
                                                                               Action<MlErrorDetails> actionFail)
        => source.ExecSelfIfFailWithoutException(actionFail).ToAsync();











    #endregion




}

