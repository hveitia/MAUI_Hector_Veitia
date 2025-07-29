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
            ErrorMessage = "Por favor, ingresa usuario y contraseña";
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
                System.Diagnostics.Debug.WriteLine($"Login exitoso - Token recibido: {response.Token[..Math.Min(response.Token.Length, 20)]}...");
                
                // Set authentication token
                _httpService.SetAuthToken(response.Token);
                System.Diagnostics.Debug.WriteLine("Token configurado en HttpService");
                
                // Verificar que el token se configuró correctamente
                if (_httpService.HasAuthToken())
                {
                    System.Diagnostics.Debug.WriteLine("✅ Verificación: Token está presente en HttpService");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ ERROR: Token NO se configuró correctamente");
                }
                
                // Navigate to customers page
                await Shell.Current.GoToAsync("//customers");
            }
            else
            {
                ErrorMessage =  "Error al iniciar sesión";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error de conexión. Intenta nuevamente.";
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}