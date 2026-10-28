
namespace MoralesLarios.OOFP.Configuration.Config;

/// <summary>
/// Contrato de <c>IMlConfigurationReader</c>.
/// </summary>
public interface IMlConfigurationReader
{
    /// <summary>
    /// Lee el contenido de <c>AppSettingKey</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="appSettingKey">Clave identificativa.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> ReadAppSettingKey<T>(string appSettingKey, MlErrorDetails errorsDetails = null!);
    /// <summary>
    /// Lee el contenido de <c>AppSettingKey</c>.
    /// </summary>
    /// <param name="appSettingKey">Clave identificativa.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<object> ReadAppSettingKey(string appSettingKey, MlErrorDetails errorsDetails = null!);
    /// <summary>
    /// Lee el contenido de <c>ConnectionStringKey</c>.
    /// </summary>
    /// <param name="connectionStringKey">Clave identificativa.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<string> ReadConnectionStringKey(string connectionStringKey, MlErrorDetails errorsDetails = null!);
    /// <summary>
    /// Lee el contenido de <c>AppSettingKey</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="appSettingKey">Clave identificativa.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> ReadAppSettingKey<T>(string appSettingKey, string errorMessage);
    /// <summary>
    /// Lee el contenido de <c>AppSettingKey</c>.
    /// </summary>
    /// <param name="appSettingKey">Clave identificativa.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<object> ReadAppSettingKey(string appSettingKey, string errorMessage);
    /// <summary>
    /// Lee el contenido de <c>ConnectionStringKey</c>.
    /// </summary>
    /// <param name="connectionStringKey">Clave identificativa.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<string> ReadConnectionStringKey(string connectionStringKey, string errorMessage);
}