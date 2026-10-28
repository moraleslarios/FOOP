// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Configuration.Config;
/// <summary>
/// Representa <c>MlConfigurationReader</c>.
/// </summary>
public class MlConfigurationReader(IConfiguration configuration) : IMlConfigurationReader
{
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Lee el contenido de <c>ConnectionStringKey</c>.
    /// </summary>
    /// <param name="connectionStringKey">Clave identificativa.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<string> ReadConnectionStringKey(string          connectionStringKey,
                                                    MlErrorDetails errorsDetails = null!)
        => ReadConfigKey(connectionStringKey, _configuration.GetConnectionString, errorsDetails)!;

    /// <summary>
    /// Lee el contenido de <c>AppSettingKey</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="appSettingKey">Clave identificativa.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<T> ReadAppSettingKey<T>(string          appSettingKey,
                                            MlErrorDetails errorsDetails = null!)
        => ReadConfigKey<T>(appSettingKey, key => _configuration.GetValue<T>(key)!, errorsDetails);

    /// <summary>
    /// Lee el contenido de <c>AppSettingKey</c>.
    /// </summary>
    /// <param name="appSettingKey">Clave identificativa.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<object> ReadAppSettingKey(string          appSettingKey,
                                              MlErrorDetails errorsDetails = null!)
        => ReadConfigKey<object>(appSettingKey, key => _configuration[key]!, errorsDetails);


    /// <summary>
    /// Lee el contenido de <c>ConnectionStringKey</c>.
    /// </summary>
    /// <param name="connectionStringKey">Clave identificativa.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<string> ReadConnectionStringKey(string connectionStringKey,
                                                    string errorMessage)
        => ReadConfigKey(connectionStringKey, _configuration.GetConnectionString, errorMessage)!;

    /// <summary>
    /// Lee el contenido de <c>AppSettingKey</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="appSettingKey">Clave identificativa.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<T> ReadAppSettingKey<T>(string appSettingKey,
                                            string errorMessage)
        => ReadConfigKey<T>(appSettingKey, key => _configuration.GetValue<T>(key)!, errorMessage);

    /// <summary>
    /// Lee el contenido de <c>AppSettingKey</c>.
    /// </summary>
    /// <param name="appSettingKey">Clave identificativa.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<object> ReadAppSettingKey(string appSettingKey,
                                              string errorMessage)
        => ReadConfigKey<object>(appSettingKey, key => _configuration[key]!, errorMessage);





    private MlResult<T> ReadConfigKey<T>(string          configKey, 
                                         Func<string, T> configSearch,
                                         MlErrorDetails errorsDetails = null!)
    {
        var result = EnsureFp.NotNullEmptyOrWhitespace(configKey, "Tkey cannot be null white or empty")
                                .Map ( _               => configSearch(configKey))
                                .MapEnsure(res => res is not null, errorsDetails is not null
                                                                    ? errorsDetails
                                                                    : $"No value found configured with the key '{configKey}'");                               
        return result!;
    }

    private MlResult<T> ReadConfigKey<T>(string          configKey, 
                                         Func<string, T> configSearch,
                                         string          errorMessage)
        => ReadConfigKey(configKey, configSearch, MlErrorDetails.FromErrorMessage(errorMessage));


}

