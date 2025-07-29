namespace MAUI_APP.Services;

public interface IHttpService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<T?> DeleteAsync<T>(string endpoint);
    void SetAuthToken(string token);
    void ClearAuthToken();
    bool HasAuthToken();
}