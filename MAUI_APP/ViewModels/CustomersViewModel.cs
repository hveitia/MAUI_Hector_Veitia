using MAUI_APP.Services;
using MAUI_APP.ViewModels;
using MAUI_APP.Models;
using System.Windows.Input;

public class CustomersViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IHttpService _httpService;

    private List<Customer> _allCustomers  = [];
    
    public CustomersViewModel(IApiService apiService, IHttpService httpService)
    {
        _apiService = apiService;
        _httpService = httpService;
        GetCustomersCommand = new Command(async () => await GetCustomersAsync(), () => !IsBusy);
        Title = "Clientes";
    }

    public ICommand GetCustomersCommand { get; }
    
    public List<Customer> AllCustomers
    {
        get => _allCustomers;
        set => SetProperty(ref _allCustomers, value);
    }
    
    public async Task GetCustomersAsync()
    {
        IsBusy = true;
        try
        {
            System.Diagnostics.Debug.WriteLine("CustomersViewModel: Iniciando carga de clientes...");
            
            // Verificar si tenemos token de autenticación
            if (!_httpService.HasAuthToken())
            {
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: NO HAY TOKEN DE AUTENTICACIÓN");
                await Application.Current.MainPage.DisplayAlert(
                    "Error de Autenticación", 
                    "No tienes permisos para acceder a esta información. Por favor, inicia sesión nuevamente.", 
                    "OK");
                    
                // Navegar de vuelta al login
                await Shell.Current.GoToAsync("//");
                return;
            }
            
            var customers = await _apiService.GetCustomersAsync();

            if (customers != null && customers.Count > 0)
            {
                AllCustomers = customers;
                System.Diagnostics.Debug.WriteLine($"CustomersViewModel: {customers.Count} clientes cargados exitosamente");
            }
            else
            {
                AllCustomers = [];
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: No se recibieron clientes o lista vacía");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CustomersViewModel Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            AllCustomers = [];
            
            // Verificar si es un error de autenticación
            if (ex.Message.Contains("401") || ex.Message.Contains("403") || ex.Message.Contains("Unauthorized"))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error de Autenticación", 
                    "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.", 
                    "OK");
                await Shell.Current.GoToAsync("//");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"No se pudieron cargar los clientes: {ex.Message}", 
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}