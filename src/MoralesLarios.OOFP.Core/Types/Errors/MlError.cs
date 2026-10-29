// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types.Errors;


/// <summary>
/// Error individual de la biblioteca, identificado por su mensaje.
/// </summary>
public record MlError
{
    /// <summary>
    /// Obtiene o establece el valor de <c>Message</c>.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MlError"/>.
    /// </summary>
    /// <param name="message">Mensaje descriptivo.</param>
    public MlError(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) Message = Constants.DefaultErrorMessage;
        else                                    Message = message;
    }

    /// <inheritdoc/>
    public override string ToString() => Message;

    /// <summary>
    /// Ejecuta <c>FromErrorMessage</c>.
    /// </summary>
    /// <param name="Message">Valor de <c>Message</c>.</param>
    /// <returns>Valor de tipo <c>MlError</c> obtenido por la operación.</returns>
    public static MlError FromErrorMessage(string Message) => new MlError(Message);

    /// <summary>
    /// Realiza la conversión implícita a <c>MlError</c>.
    /// </summary>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Valor de tipo <c>MlError</c> obtenido por la operación.</returns>
    public static implicit operator MlError(string message) => new(message);

}


/// <summary>
/// Métodos de extensión para crear instancias de <see cref="MlError"/> y colecciones de errores a partir de cadenas de texto.
/// </summary>
public static class MlErrorExtensions
{

    /// <summary>
    /// Ejecuta <c>ToMlError</c>.
    /// </summary>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Valor de tipo <c>MlError</c> obtenido por la operación.</returns>
    public static MlError ToMlError(this string message) => new(message);
    /// <summary>
    /// Ejecuta <c>ToMlErrors</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    public static IEnumerable<MlError> ToMlErrors(this MlError error) => new List<MlError> { error };

    /// <summary>
    /// Ejecuta <c>ToMlErrors</c>.
    /// </summary>
    /// <param name="messages">Mensajes descriptivos.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    public static IEnumerable<MlError> ToMlErrors(this IEnumerable<string> messages) => messages.Select(x => x.ToMlError());
    /// <summary>
    /// Ejecuta <c>ToMlErrors</c>.
    /// </summary>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    public static IEnumerable<MlError> ToMlErrors(this string message) => new List<MlError> { message.ToMlError() };

}


