// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types.Errors;
/// <summary>
/// Representa <c>MlErrorDetailsActions</c>.
/// </summary>
public static partial class MlErrorDetailsActions
{
    /// <summary>
    /// Ejecuta <c>HasKeyDetails</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    [global::System.Obsolete("Renamed to ContainsMetadata. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static bool HasKeyDetails(this MlErrorDetails source, string key) => ContainsMetadata(source, key);
    /// <summary>
    /// Ejecuta <c>HasValueDetails</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    [global::System.Obsolete("Renamed to ContainsValueMetadata. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static bool HasValueDetails(this MlErrorDetails source) => ContainsValueMetadata(source);
    /// <summary>
    /// Ejecuta <c>HasValueDetails</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el <c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    [global::System.Obsolete("Renamed to ContainsValueMetadataAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<bool> HasValueDetailsAsync(this MlErrorDetails source) => ContainsValueMetadataAsync(source);
    /// <summary>
    /// Ejecuta <c>HasExceptionDetails</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    [global::System.Obsolete("Renamed to ContainsException. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static bool HasExceptionDetails(this MlErrorDetails source) => ContainsException(source);
    /// <summary>
    /// Ejecuta <c>HasExceptionDetails</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el <c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    [global::System.Obsolete("Renamed to ContainsExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<bool> HasExceptionDetailsAsync(this MlErrorDetails source) => ContainsExceptionAsync(source);
    /// <summary>
    /// Añade el elemento indicado de <c>Detail</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddMetadataOrThrow. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails AddDetail<T>(this MlErrorDetails source, string key, T value) => AddMetadataOrThrow<T>(source, key, value);
    /// <summary>
    /// Añade el elemento indicado de <c>Details</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherDetails">Valor de <c>otherDetails</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddMetadataRange. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails AddDetails(this MlErrorDetails source, Dictionary<string, object> otherDetails) => AddMetadataRange(source, otherDetails);
    /// <summary>
    /// Añade el elemento indicado de <c>Details</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherDetails">Colección de <c>otherDetails</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddMetadataRange. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails AddDetails(this MlErrorDetails source, params (string key, object value)[] otherDetails) => AddMetadataRange(source, otherDetails);
    /// <summary>
    /// Añade el elemento indicado de <c>DetailValue</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddValueMetadata. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails AddDetailValue<T>(this MlErrorDetails source, T value) => AddValueMetadata<T>(source, value);
}