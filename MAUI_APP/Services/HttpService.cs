using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MAUI_APP.Models;

namespace MAUI_APP.Services;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
   
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{endpoint}");
            System.Diagnostics.Debug.WriteLine($"Authorization Header: {_httpClient.DefaultRequestHeaders.Authorization}");
            
            var response = await _httpClient.GetAsync(endpoint);
            
            System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error Response: {errorContent}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("Error 401/403: Token de autenticación inválido o faltante");
                }
                
                response.EnsureSuccessStatusCode(); // Esto lanzará la excepción con detalles
            }

            var content = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Response Content: {content}");
            
            // Intentar deserialización directa primero
            try
            {
                var result = JsonSerializer.Deserialize<T>(content);
                System.Diagnostics.Debug.WriteLine("✅ Deserialización directa exitosa");
                return result;
            }
            catch (JsonException)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Deserialización directa falló, intentando doble deserialización...");
                // Si falla, intentar doble deserialización (para APIs que devuelven JSON como string)
                try
                {
                    string innerJson = JsonSerializer.Deserialize<string>(content);
                    var result = JsonSerializer.Deserialize<T>(innerJson);
                    System.Diagnostics.Debug.WriteLine("✅ Doble deserialización exitosa");
                    return result;
                }
                catch (JsonException jsonEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error en doble deserialización: {jsonEx.Message}");
                    throw;
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine($"HTTP Error: {httpEx.Message}");
            System.Diagnostics.Debug.WriteLine($"HTTP Error Data: {httpEx.Data}");
            throw; // Re-lanzar para que el llamador pueda manejar el error específico
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GET Error: {ex.Message}");
            throw; // Re-lanzar para que el llamador pueda manejar el error
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            
            string jsonResponse = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"POST Response Content: {jsonResponse}");
            
            // Intentar deserialización directa primero
            try
            {
                var result = JsonSerializer.Deserialize<T>(jsonResponse);
                System.Diagnostics.Debug.WriteLine("✅ POST Deserialización directa exitosa");
                return result;
            }
            catch (JsonException)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ POST Deserialización directa falló, intentando doble deserialización...");
                // Si falla, intentar doble deserialización
                try
                {
                    string innerJson = JsonSerializer.Deserialize<string>(jsonResponse);
                    var result = JsonSerializer.Deserialize<T>(innerJson);
                    System.Diagnostics.Debug.WriteLine("✅ POST Doble deserialización exitosa");
                    return result;
                }
                catch (JsonException jsonEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error en POST doble deserialización: {jsonEx.Message}");
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"POST Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"PUT Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> DeleteAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            return result;
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"DELETE Error: {ex.Message}");
            return default;
        }
    }

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthToken()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public bool HasAuthToken()
    {
        return _httpClient.DefaultRequestHeaders.Authorization != null 
               && !string.IsNullOrEmpty(_httpClient.DefaultRequestHeaders.Authorization.Parameter);
    }
}