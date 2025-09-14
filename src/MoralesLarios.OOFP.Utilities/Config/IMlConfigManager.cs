
namespace MoralesLarios.OOFP.Utilities.Config;

public interface IMlConfigManager
{
    MlResult<T> ReadAppSettingKey<T>(string appSettingKey);
    MlResult<object> ReadAppSettingKey(string appSettingKey);
    MlResult<string> ReadConnectionStringKey(string connectionStringKey);
}