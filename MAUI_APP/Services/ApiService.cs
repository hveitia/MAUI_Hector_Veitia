using MAUI_APP.Models;

namespace MAUI_APP.Services;

public class ApiService : IApiService
{
    private readonly IHttpService _httpService;

    public ApiService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpService.PostAsync<LoginResponse>("Authentication/Authenticate", request);
        return response ?? new LoginResponse();
    
    }

    public async Task<ApiResponse<User>> GetUserAsync(int userId)
    {
        var response = await _httpService.GetAsync<ApiResponse<User>>($"users/{userId}");
        return response ?? new ApiResponse<User> 
        { 
            Success = false, 
            Error = "Error al obtener el usuario" 
        };
    }

    public async Task<ApiResponse<List<User>>> GetUsersAsync()
    {
        var response = await _httpService.GetAsync<ApiResponse<List<User>>>("users");
        return response ?? new ApiResponse<List<User>> 
        { 
            Success = false, 
            Error = "Error al obtener los usuarios" 
        };
    }

    public async Task<ApiResponse<User>> CreateUserAsync(User user)
    {
        var response = await _httpService.PostAsync<ApiResponse<User>>("users", user);
        return response ?? new ApiResponse<User> 
        { 
            Success = false, 
            Error = "Error al crear el usuario" 
        };
    }

    public async Task<ApiResponse<User>> UpdateUserAsync(int userId, User user)
    {
        var response = await _httpService.PutAsync<ApiResponse<User>>($"users/{userId}", user);
        return response ?? new ApiResponse<User> 
        { 
            Success = false, 
            Error = "Error al actualizar el usuario" 
        };
    }

    public async Task<ApiResponse> DeleteUserAsync(int userId)
    {
        var response = await _httpService.DeleteAsync<ApiResponse>($"users/{userId}");
        return response ?? new ApiResponse 
        { 
            Success = false, 
            Error = "Error al eliminar el usuario" 
        };
    }
}