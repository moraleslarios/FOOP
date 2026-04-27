// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Utilities.Config;
public class MlConfigManager(IConfiguration configuration) : IMlConfigManager
{
    private readonly IConfiguration _configuration = configuration;

    public MlResult<string> ReadConnectionStringKey(string          connectionStringKey,
                                                    MlErrorsDetails errorsDetails = null!)
        => ReadConfigKey(connectionStringKey, _configuration.GetConnectionString, errorsDetails)!;

    public MlResult<T> ReadAppSettingKey<T>(string          appSettingKey,
                                            MlErrorsDetails errorsDetails = null!)
        => ReadConfigKey<T>(appSettingKey, key => _configuration.GetValue<T>(key)!, errorsDetails);

    public MlResult<object> ReadAppSettingKey(string          appSettingKey,
                                              MlErrorsDetails errorsDetails = null!)
        => ReadConfigKey<object>(appSettingKey, key => _configuration[key]!, errorsDetails);


    public MlResult<string> ReadConnectionStringKey(string connectionStringKey,
                                                    string errorMessage)
        => ReadConfigKey(connectionStringKey, _configuration.GetConnectionString, errorMessage)!;

    public MlResult<T> ReadAppSettingKey<T>(string appSettingKey,
                                            string errorMessage)
        => ReadConfigKey<T>(appSettingKey, key => _configuration.GetValue<T>(key)!, errorMessage);

    public MlResult<object> ReadAppSettingKey(string appSettingKey,
                                              string errorMessage)
        => ReadConfigKey<object>(appSettingKey, key => _configuration[key]!, errorMessage);





    private MlResult<T> ReadConfigKey<T>(string          configKey, 
                                         Func<string, T> configSearch,
                                         MlErrorsDetails errorsDetails = null!)
    {
        var result = EnsureFp.NotNullEmptyOrWhitespace(configKey, "Tkey cannot be null white or empty")
                                .Map ( _               => configSearch(configKey))
                                .Bind(configSearchData => errorsDetails is not null 
                                                                ? configSearchData.NullToFailed($"No value found configured with the key '{configKey}'")
                                                                : configSearchData.NullToFailed(errorsDetails!));
        return result!;
    }

    private MlResult<T> ReadConfigKey<T>(string          configKey, 
                                         Func<string, T> configSearch,
                                         string          errorMessage)
        => ReadConfigKey(configKey, configSearch, MlErrorsDetails.FromErrorMessage(errorMessage));


}

