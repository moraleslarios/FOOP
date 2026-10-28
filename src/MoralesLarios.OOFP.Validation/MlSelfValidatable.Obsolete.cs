// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Validation;

/// <summary>
/// Clase base de autovalidacion. Sustituida por la interfaz <see cref="IMlSelfValidatable{T}"/>,
/// que no consume la unica posicion de clase base del consumidor.
/// </summary>
[global::System.Obsolete("Replaced by the interface IMlSelfValidatable<T>. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public abstract class MlSelfValidatable<T> : IMlSelfValidatable<T>
    where T : class
{

    /// <summary>
    /// Válida el valor.
    /// </summary>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public abstract MlResult<T> Validate();

}
