// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


namespace MoralesLarios.OOFP.Application.Services;

/// <summary>
/// Contrato de <c>ICrudServiceFp</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
public interface ICrudServiceFp<TEntity, TDto>
    where TEntity  : class
    where TDto     : class
{
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Paged</c>. Versión asíncrona.
    /// </summary>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<IEnumerable<TDto>>> GetPagedAsync(CancellationToken               ct                  = default!,
                                               string                          initialMessage      = null!,
                                               Func<IEnumerable<TDto>, string> successMessageFactory = null!,
                                               Func<MlErrorDetails, string>   failMessageBuilder  = null!);

    /// <summary>
    /// Busca el elemento solicitado de <c>ByIdProblemsDetails</c>. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto?>> FindByIdProblemsDetailsAsync(MlErrorDetails               notFoundErrorDetails,
                                                       CancellationToken             ct                   = default, 
                                                       string                        initialMessage       = null!, 
                                                       Func<TDto, string>            successMessageFactory  = null!, 
                                                       Func<MlErrorDetails, string> failMessageBuilder   = null!, 
                                                       params object[] pk);


    /// <summary>
    /// Busca el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto?>> FindByIdAsync(CancellationToken             ct                  = default!,
                                        string                        initialMessage      = null!,
                                        Func<TDto, string>            successMessageFactory = null!,
                                        Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                        params object[] pk);

    /// <summary>
    /// Crea la instancia solicitada. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> CreateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            successMessageFactory = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!);

    /// <summary>
    /// Actualiza el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            successMessageFactory = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                     params object[] pk);

    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> DeleteAsync(CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                     params object[] pk);

    /// <summary>
    /// Actualiza el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            successMessageFactory = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!);

    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> DeleteAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!);
    /// <summary>
    /// Actualiza el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> UpdateAsync(TDto dto, MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<TDto, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    /// <summary>
    /// Elimina el elemento indicado de <c>ProblemDetails</c>. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> DeleteProblemDetailsAsync(MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
}


/// <summary>
/// Contrato de <c>ICrudServiceFp</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
public interface ICrudServiceFp<TEntity, TRequest, TResponse>
    where TEntity : class
    where TRequest : class
    where TResponse : class
{
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Paged</c>. Versión asíncrona.
    /// </summary>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<IEnumerable<TResponse>>> GetPagedAsync(CancellationToken ct = default, string initialMessage = null, Func<IEnumerable<TResponse>, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null);
    /// <summary>
    /// Crea la instancia solicitada. Versión asíncrona.
    /// </summary>
    /// <param name="dtoRequest">Valor de <c>dtoRequest</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> CreateAsync(TRequest dtoRequest, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null);
    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> DeleteAsync(CancellationToken ct = default, string initialMessage = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dtoRequest">Valor de <c>dtoRequest</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> DeleteAsync(TRequest dtoRequest, CancellationToken ct = default, string initialMessage = null, Func<MlErrorDetails, string> failMessageBuilder = null);
    /// <summary>
    /// Elimina el elemento indicado de <c>ProblemDetails</c>. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> DeleteProblemDetailsAsync(MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    /// <summary>
    /// Busca el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse?>> FindByIdAsync(CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    /// <summary>
    /// Busca el elemento solicitado de <c>ByIdProblemsDetails</c>. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse?>> FindByIdProblemsDetailsAsync(MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    /// <summary>
    /// Actualiza el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dtoRequest">Valor de <c>dtoRequest</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> UpdateAsync(TRequest dtoRequest, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    /// <summary>
    /// Actualiza el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dtoRequest">Valor de <c>dtoRequest</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> UpdateAsync(TRequest dtoRequest, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null);
    /// <summary>
    /// Actualiza el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dtoRequest">Valor de <c>dtoRequest</c>.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="initialMessage">Mensaje descriptivo.</param>
    /// <param name="successMessageFactory">Función que crea la instancia solicitada.</param>
    /// <param name="failMessageBuilder">Función a aplicar.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> UpdateAsync(TRequest dtoRequest, MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
}
