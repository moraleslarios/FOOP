
namespace MoralesLarios.OOFP.Configuration.Config;

public interface IMlConfigurationReader
{
    MlResult<T> ReadAppSettingKey<T>(string appSettingKey, MlErrorDetails errorsDetails = null!);
    MlResult<object> ReadAppSettingKey(string appSettingKey, MlErrorDetails errorsDetails = null!);
    MlResult<string> ReadConnectionStringKey(string connectionStringKey, MlErrorDetails errorsDetails = null!);
    MlResult<T> ReadAppSettingKey<T>(string appSettingKey, string errorMessage);
    MlResult<object> ReadAppSettingKey(string appSettingKey, string errorMessage);
    MlResult<string> ReadConnectionStringKey(string connectionStringKey, string errorMessage);
}