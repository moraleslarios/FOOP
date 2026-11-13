namespace MoralesLarios.OOFP.Types.Errors;

/// <summary>
/// Excepción que indica que se intentó obtener el valor de un <see cref="MlResult{T}"/> fallido.
/// </summary>
public sealed class MlResultFailureException : InvalidOperationException
{
    internal MlResultFailureException(MlErrorDetails errorDetails, string message)
        : base(message)
    {
        ErrorDetails = errorDetails ?? throw new ArgumentNullException(nameof(errorDetails));
    }

    /// <summary>
    /// Detalles del fallo que provocó la excepción.
    /// </summary>
    public MlErrorDetails ErrorDetails { get; }
}