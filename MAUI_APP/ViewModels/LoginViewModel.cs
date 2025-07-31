using System.Windows.Input;
using MAUI_APP.Models;
using MAUI_APP.Services;

namespace MAUI_APP.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IHttpService _httpService;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public LoginViewModel(IApiService apiService, IHttpService httpService)
    {
        _apiService = apiService;
        _httpService = httpService;
        LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
        Title = "Login";
        
        PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(IsBusy))
            {
                ((Command)LoginCommand).ChangeCanExecute();
            }
        };
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter username and password";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var loginRequest = new LoginRequest
            {
                Username = Username,
                Password = Password
            };

            var response = await _apiService.LoginAsync(loginRequest);

            if (response.Oid != null && !string.IsNullOrEmpty(response.Token))
            {
                System.Diagnostics.Debug.WriteLine($"Login successful - Token received: {response.Token[..Math.Min(response.Token.Length, 20)]}...");
            
                // Set authentication token
                _httpService.SetAuthToken(response.Token);
                System.Diagnostics.Debug.WriteLine("Token configured in HttpService");
            
                // Verify token was set correctly
                if (_httpService.HasAuthToken())
                {
                    System.Diagnostics.Debug.WriteLine("✅ Verification: Token is present in HttpService");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ ERROR: Token was NOT configured correctly");
                }
            
                // Navigate to customers page
                await Shell.Current.GoToAsync("//customers");
            }
            else
            {
                ErrorMessage = "Login error";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Connection error. Please try again.";
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}