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
            Error = "Error getting user"
        };
    }

    public async Task<ApiResponse<List<User>>> GetUsersAsync()
    {
        var response = await _httpService.GetAsync<ApiResponse<List<User>>>("users");
        return response ?? new ApiResponse<List<User>>
        {
            Success = false,
            Error = "Error getting users"
        };
    }

    public async Task<ApiResponse<User>> CreateUserAsync(User user)
    {
        var response = await _httpService.PostAsync<ApiResponse<User>>("users", user);
        return response ?? new ApiResponse<User>
        {
            Success = false,
            Error = "Error creating user"
        };
    }

    public async Task<ApiResponse<User>> UpdateUserAsync(int userId, User user)
    {
        var response = await _httpService.PutAsync<ApiResponse<User>>($"users/{userId}", user);
        return response ?? new ApiResponse<User>
        {
            Success = false,
            Error = "Error updating user"
        };
    }

    public async Task<ApiResponse> DeleteUserAsync(int userId)
    {
        var response = await _httpService.DeleteAsync<ApiResponse>($"users/{userId}");
        return response ?? new ApiResponse
        {
            Success = false,
            Error = "Error deleting user"
        };
    }
    
    public async Task<List<Customer>> GetCustomersAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Starting GetCustomersAsync...");
            
            // Call OData endpoint that returns the complete structure
            var odataResponse = await _httpService.GetAsync<ODataResponse<Customer>>("odata/Customer");
            
            if (odataResponse?.Value != null)
            {
                System.Diagnostics.Debug.WriteLine($"OData response received: {odataResponse.Value.Count} customers");
                System.Diagnostics.Debug.WriteLine($"Context: {odataResponse.Context}");
                return odataResponse.Value;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OData response null or without value");
                return [];
            }
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine($"HTTP error in GetCustomersAsync: {httpEx.Message}");
            // Check if it's an authentication problem
            if (httpEx.Message.Contains("401") || httpEx.Message.Contains("403"))
            {
                System.Diagnostics.Debug.WriteLine("Authentication error - Invalid or expired token");
            }
            return [];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"General error in GetCustomersAsync: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            return [];
        }
    }
    
    public async Task<object> UpdateCustomerAsync(string oid, Customer customer)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Starting UpdateCustomerAsync for OID: {oid}");
            
            // Call OData endpoint to update the customer
            var response = await _httpService.PutAsync<Customer>($"odata/Customer({oid})", customer);
            
            if (response != null)
            {
                System.Diagnostics.Debug.WriteLine($"Customer updated successfully: {oid}");
                return response;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Null response when updating customer: {oid}");
                return false;
            }
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine($"HTTP error in UpdateCustomerAsync: {httpEx.Message}");
            
            if (httpEx.Message.Contains("401") || httpEx.Message.Contains("403"))
            {
                System.Diagnostics.Debug.WriteLine("Authentication error - Invalid or expired token");
                return false;
            }
            else if (httpEx.Message.Contains("404"))
            {
                System.Diagnostics.Debug.WriteLine("Customer not found");
                return false;
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"General error in UpdateCustomerAsync: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            
            return false;
        }
    }
    
    public async Task<object> DeleteCustomerAsync(string oid)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Starting DeleteCustomerAsync for OID: {oid}");
            
            // Call OData endpoint to delete the customer
            var response = await _httpService.DeleteAsync<ODataResponse<object>>($"odata/Customer({oid})");
            
            if (response?.Value != null)
            {
                System.Diagnostics.Debug.WriteLine($"Customer deleted successfully: {oid}");
                return response.Value;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Null response when deleting customer: {oid}");
                return false;
            }
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine($"HTTP error in DeleteCustomerAsync: {httpEx.Message}");
            
            if (httpEx.Message.Contains("401") || httpEx.Message.Contains("403"))
            {
                System.Diagnostics.Debug.WriteLine("Authentication error - Invalid or expired token");
                return false;
            }
            else if (httpEx.Message.Contains("404"))
            {
                return false;
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"General error in DeleteCustomerAsync: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            
            return false;
        }
    }
}