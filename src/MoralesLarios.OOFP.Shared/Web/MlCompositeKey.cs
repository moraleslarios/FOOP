namespace MoralesLarios.OOFP.Shared.Web;

/// <summary>
/// Convenciones compartidas para claves primarias compuestas en rutas HTTP.
/// </summary>
public static class MlCompositeKey
{
    /// <summary>
    /// Separador literal de componentes. Codifica cada componente antes de unirlos y decodifícalos después de partir.
    /// </summary>
    public const char Separator = ',';
}