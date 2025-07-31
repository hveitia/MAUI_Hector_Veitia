using MAUI_APP.Models;

namespace MAUI_APP.Services;

public interface IApiService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<ApiResponse<User>> GetUserAsync(int userId);
    Task<ApiResponse<List<User>>> GetUsersAsync();
    Task<ApiResponse<User>> CreateUserAsync(User user);
    Task<ApiResponse<User>> UpdateUserAsync(int userId, User user);
    Task<List<Customer>> GetCustomersAsync();
    Task<object> UpdateCustomerAsync(string oid, Customer customer);
    Task<bool> DeleteCustomerAsync(string oid);
}