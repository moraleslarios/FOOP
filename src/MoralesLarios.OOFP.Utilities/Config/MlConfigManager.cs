




namespace MoralesLarios.OOFP.Utilities.Config;
public class MlConfigManager(IConfiguration configuration) : IMlConfigManager
{
    private readonly IConfiguration _configuration = configuration;

    public MlResult<string> ReadConnectionStringKey(string connectionStringKey)
        => ReadConfigKey(connectionStringKey, _configuration.GetConnectionString)!;

    public MlResult<T> ReadAppSettingKey<T>(string appSettingKey)
        => ReadConfigKey<T>(appSettingKey, key => _configuration.GetValue<T>(key)!);

    public MlResult<object> ReadAppSettingKey(string appSettingKey)
        => ReadConfigKey<object>(appSettingKey, key => _configuration[key]!);



    private MlResult<T> ReadConfigKey<T>(string configKey, Func<string, T> configSearch)
    {
        var result = EnsureFp.NotNullEmptyOrWhitespace(configKey, "Tkey cannot be null white or empty")
                                .Map ( _                => configSearch(configKey))
                                .Bind( configSearchData => configSearchData.NullToFailed($"No value found configured with the key '{configKey}'"));
        return result!;
    }


}
