
namespace MoralesLarios.OOFP.Utilities.Config;

public interface IMlConfigManager
{
    MlResult<T> ReadAppSettingKey<T>(string appSettingKey, MlErrorsDetails errorsDetails = null!);
    MlResult<object> ReadAppSettingKey(string appSettingKey, MlErrorsDetails errorsDetails = null!);
    MlResult<string> ReadConnectionStringKey(string connectionStringKey, MlErrorsDetails errorsDetails = null!);
    MlResult<T> ReadAppSettingKey<T>(string appSettingKey, string errorMessage);
    MlResult<object> ReadAppSettingKey(string appSettingKey, string errorMessage);
    MlResult<string> ReadConnectionStringKey(string connectionStringKey, string errorMessage);
}