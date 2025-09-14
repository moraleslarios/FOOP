namespace MoralesLarios.OOFP.WebApi.Helpers;

/// <summary>
/// Provides extension methods for converting <see cref="MlResult{T}"/> results to ASP.NET Core <see cref="IActionResult"/> types for Web API responses.
/// </summary>
public static class MlResultWebExtensions
{

    private static IEnumerable<string> notFoundKeys = ["NotFound", "Not Found", "Not_Found", "not found", "NoEncontrado", "No Encontrado", "No_Encontrado", "no encontrado",
                                                        "No se han encontrado", "no se han encontrado"];


    /// <summary>
    /// Converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a GET request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static IActionResult ToRepoGetActionResult<T>(this MlResult<T>    source, 
                                                              ControllerBase controllerBase)
        => source.ToRepoActionResult(controllerBase, () => controllerBase.Ok(source.SecureValidValue()));

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a GET request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static Task<IActionResult> ToRepoGetActionResultAsync<T>(this MlResult<T>    source, 
                                                                         ControllerBase controllerBase)
        => source.ToRepoActionResult(controllerBase, () => controllerBase.Ok(source.SecureValidValue())).ToAsync();

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> from a task to an <see cref="IActionResult"/> for a GET request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="sourceAsync">The source result as a task.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static async Task<IActionResult> ToRepoGetActionResultAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                               ControllerBase    controllerBase)
    {
        MlResult<T> source = await sourceAsync;

        return source.ToRepoActionResult(controllerBase, () => controllerBase.Ok(source.SecureValidValue()));
    }

    /// <summary>
    /// Converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a PUT request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static IActionResult ToRepoPutActionResult<T>(this MlResult<T>    source, 
                                                              ControllerBase controllerBase)
        => source.ToRepoActionResult(controllerBase, () => controllerBase.NoContent());

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a PUT request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="sourceAsync">The source result as a task.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static async Task<IActionResult> ToRepoPutActionResultAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                               ControllerBase    controllerBase)
        => (await sourceAsync).ToRepoPutActionResult(controllerBase);

    /// <summary>
    /// Converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a POST request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <param name="getMethodName">The name of the GET method for routing.</param>
    /// <param name="pkSelector">A function to select the primary key from the result.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static IActionResult ToRepoPostActionResult<T>(this MlResult<T>     source,
                                                               ControllerBase  controllerBase,
                                                               string          getMethodName,
                                                               Func<T, object> pkSelector)
    {
        var result = EnsureFp.NotNullEmptyOrWhitespace(getMethodName, "El nombre del método GET no puede ser nulo, vacío o espacio en blanco")
                                .Match(valid: _      => source.ToRepoActionResult(controllerBase: controllerBase,
                                                                                  OkHandler     : () => controllerBase.CreatedAtRoute(getMethodName,
                                                                                                                                        source.IsValid
                                                                                                                                                ? pkSelector(source.SecureValidValue())
                                                                                                                                                : null!,
                                                                                                                                        source.SecureValidValue())).ToMlResultValid(),
                                        fail: errors => (new BadRequestObjectResult(errors.ToErrorsDescription()) as IActionResult).ToMlResultValid())
                                .SecureValidValue();
        return result;
    }

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a POST request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <param name="getMethodName">The name of the GET method for routing.</param>
    /// <param name="pkSelector">A function to select the primary key from the result.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static Task<IActionResult> ToRepoPostActionResultAsync<T>(this MlResult<T>     source,
                                                                          ControllerBase  controllerBase,
                                                                          string          getMethodName,
                                                                          Func<T, object> pkSelector)
        => source.ToRepoPostActionResult(controllerBase, getMethodName, pkSelector).ToAsync();



    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> from a task to an <see cref="IActionResult"/> for a POST request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="sourceAsync">The source result as a task.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <param name="getMethodName">The name of the GET method for routing.</param>
    /// <param name="pkSelector">A function to select the primary key from the result.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static async Task<IActionResult> ToRepoPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                ControllerBase    controllerBase,
                                                                                string            getMethodName,
                                                                                Func<T, object>   pkSelector)
        => await (await sourceAsync).ToRepoPostActionResultAsync(controllerBase, getMethodName, pkSelector);


    /// <summary>
    /// Converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a simple POST request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static IActionResult ToRepoPostActionResult<T>(this MlResult<T>    source, 
                                                               ControllerBase controllerBase)
        => source.ToRepoActionResult(controllerBase, () => controllerBase.Created("NotUri", new object()));

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a simple POST request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static Task<IActionResult> ToRepoPostActionResultAsync<T>(this MlResult<T>    source, 
                                                                          ControllerBase controllerBase)
        => source.ToRepoActionResult(controllerBase, () => controllerBase.Created("NotUri", new object())).ToAsync();

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> from a task to an <see cref="IActionResult"/> for a simple POST request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="sourceAsync">The source result as a task.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static Task<IActionResult> ToRepoPostActionResult<T>(this Task<MlResult<T>> sourceAsync, 
                                                                     ControllerBase    controllerBase)
        => sourceAsync.ToRepoActionResultAsync(controllerBase, () => controllerBase.Created("NotUri", new object()));


    /// <summary>
    /// Converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a simple POST request in a repository, with handling for valid and error cases.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static IActionResult ToSimpleRepoPostActionResult<T>(this MlResult<T>    source,
                                                                          ControllerBase controllerBase)
        => source.Match(
                            valid: x      => source.ToRepoActionResult(controllerBase, () => controllerBase.Created("NotUri", x)),
                            fail : errors => source.ToRepoActionResult(controllerBase, () => controllerBase.Created("NotUri", new object()))
                        );

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a simple POST request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static Task<IActionResult> ToSimpleRepoPostActionResultAsync<T>(this MlResult<T> source,
                                                                                ControllerBase controllerBase)
        => source.ToSimpleRepoPostActionResult(controllerBase).ToAsync();

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> from a task to an <see cref="IActionResult"/> for a simple POST request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="sourceAsync">The source result as a task.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static async Task<IActionResult> ToSimpleRepoPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                      ControllerBase    controllerBase)
        => await sourceAsync.MatchAsync(
                            validAsync: x      => sourceAsync.ToRepoActionResultAsync(controllerBase, () => controllerBase.Created("NotUri", x)),
                            failAsync : errors => sourceAsync.ToRepoActionResultAsync(controllerBase, () => controllerBase.Created("NotUri", new object()))
                        );


    /// <summary>
    /// Converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a DELETE request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static IActionResult ToRepoDeleteActionResult<T>(this MlResult<T>    source, 
                                                                 ControllerBase controllerBase)
        => source.ToRepoActionResult(controllerBase, () => controllerBase.NoContent());

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for a DELETE request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static Task<IActionResult> ToRepoDeleteActionResultAsync<T>(this MlResult<T>    source, 
                                                                            ControllerBase controllerBase)
        => source.ToRepoActionResult(controllerBase, () => controllerBase.NoContent()).ToAsync();

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> from a task to an <see cref="IActionResult"/> for a DELETE request in a repository.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="sourceAsync">The source result as a task.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static async Task<IActionResult> ToRepoDeleteActionResultAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                                  ControllerBase    controllerBase)
        => await (await sourceAsync).ToRepoDeleteActionResultAsync(controllerBase);



    /// <summary>
    /// Converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for repository actions, handling both valid and error cases.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <param name="OkHandler">A function to handle the OK response.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static IActionResult ToRepoActionResult<T>(this MlResult<T>         source,
                                                           ControllerBase      controllerBase,
                                                           Func<IActionResult> OkHandler)
    {
        var badRequest = EnsureFp.NotNull(controllerBase, $"{nameof(controllerBase)} no puede ser nulo")
                                    .MapIfFail<ControllerBase, IActionResult>(funcValid: x      => controllerBase.Ok(x),
                                                                              funcFail : errors => new BadRequestObjectResult(errors.ToErrorsDescription()));

        if (controllerBase is null) return badRequest.SecureValidValue();

        if (source.IsValid) return OkHandler();

        var errorsDetails = source.SecureFailErrorsDetails();

        var errorResult = errorsDetails switch
        {
            { } errors when ContieneCombinacion(errors.ToErrorsDescription()!, notFoundKeys.ToArray()) => new NotFoundObjectResult(errors),
            { } errors when errors.HasKeyDetails("NotFound")                                           => new NotFoundObjectResult(errors),
            { } errors when errors.HasExceptionDetails()                                               => new ObjectResult(errors.ToErrorsDetailsDescription()) { StatusCode = 500 },
            _ => new ObjectResult(errorsDetails.ToErrorsDescription()) { StatusCode = 500 }
        };

        return errorResult;
    }

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> to an <see cref="IActionResult"/> for repository actions.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <param name="OkHandler">A function to handle the OK response.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static Task<IActionResult> ToRepoActionResultAsync<T>(this MlResult<T>         source,
                                                                      ControllerBase      controllerBase,
                                                                      Func<IActionResult> OkHandler)
        => source.ToRepoActionResult(controllerBase, OkHandler).ToAsync();

    /// <summary>
    /// Asynchronously converts the <see cref="MlResult{T}"/> from a task to an <see cref="IActionResult"/> for repository actions.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="sourceAsync">The source result as a task.</param>
    /// <param name="controllerBase">The controller context.</param>
    /// <param name="OkHandler">A function to handle the OK response.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the result of the operation.</returns>
    public static async Task<IActionResult> ToRepoActionResultAsync<T>(this Task<MlResult<T>>   sourceAsync,
                                                                            ControllerBase      controllerBase,
                                                                            Func<IActionResult> OkHandler)
        => await (await sourceAsync).ToRepoActionResultAsync(controllerBase, OkHandler);



    static bool ContieneCombinacion(string cadena, string[] combinaciones)
    {
        foreach (string combinacion in combinaciones)
        {
            if (cadena.IndexOf(combinacion, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }
        return false;
    }





}
