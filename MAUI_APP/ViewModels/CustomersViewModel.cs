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
        DeleteCustomerCommand = new Command<string>(async (oid) => await DeleteCustomerAsync(oid), (oid) => !IsBusy && !string.IsNullOrEmpty(oid));
        Title = "Customers";
    }

    public ICommand GetCustomersCommand { get; }

    public ICommand DeleteCustomerCommand { get; }

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
            System.Diagnostics.Debug.WriteLine("CustomersViewModel: Starting customer loading...");
            
            // Verificar si tenemos token de autenticación
            if (!_httpService.HasAuthToken())
            {
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: NO AUTHENTICATION TOKEN");
                await Application.Current.MainPage.DisplayAlert(
                    "Authentication Error", 
                    "You don't have permission to access this information. Please log in again.", 
                    "OK");
                    
                // Navegar de vuelta al login
                await Shell.Current.GoToAsync("//");
                return;
            }
            
            var customers = await _apiService.GetCustomersAsync();

            if (customers != null && customers.Count > 0)
            {
                AllCustomers = customers;
                System.Diagnostics.Debug.WriteLine($"CustomersViewModel: {customers.Count} customers loaded successfully");
            }
            else
            {
                AllCustomers = [];
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: No customers received or empty list");
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
                    "Authentication Error", 
                    "Your session has expired. Please log in again.", 
                    "OK");
                await Shell.Current.GoToAsync("//");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"Could not load customers: {ex.Message}", 
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task DeleteCustomerAsync(string oid)
    {
        if (string.IsNullOrEmpty(oid))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error", 
                "Invalid customer ID.", 
                "OK");
            return;
        }

        IsBusy = true;
        try
        {
            System.Diagnostics.Debug.WriteLine($"CustomersViewModel: Starting customer deletion with OID: {oid}");
            
            // Buscar el cliente en la lista local para mostrar su nombre en la confirmación
            var customerToDelete = AllCustomers.FirstOrDefault(c => c.Oid == oid);
            string customerName = customerToDelete?.FullName ?? "Unknown customer";
            
            // Mostrar confirmación al usuario
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Deletion",
                $"Are you sure you want to delete {customerName}? This action cannot be undone.",
                "Delete",
                "Cancel");
                
            if (!confirm)
            {
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: Deletion cancelled by user");
                return;
            }
            
            // Verificar si tenemos token de autenticación
            if (!_httpService.HasAuthToken())
            {
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: NO AUTHENTICATION TOKEN for deletion");
                await Application.Current.MainPage.DisplayAlert(
                    "Authentication Error", 
                    "You don't have permission to perform this action. Please log in again.", 
                    "OK");
                    
                await Shell.Current.GoToAsync("//");
                return;
            }
            
            // Llamar al servicio API para eliminar el cliente
            var result = await _apiService.DeleteCustomerAsync(oid);
            
            if (result != null && result is bool success && success)
            {
                System.Diagnostics.Debug.WriteLine($"CustomersViewModel: Customer deleted successfully: {oid}");
                
                // Remover el cliente de la lista local
                if (customerToDelete != null)
                {
                    var updatedList = AllCustomers.ToList();
                    updatedList.Remove(customerToDelete);
                    AllCustomers = updatedList;
                }
                
                await Application.Current.MainPage.DisplayAlert(
                    "Success", 
                    $"{customerName} has been deleted successfully.", 
                    "OK");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"CustomersViewModel: Error deleting customer");
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"Could not delete customer ", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CustomersViewModel Deletion error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            
            // Verificar si es un error de autenticación
            if (ex.Message.Contains("401") || ex.Message.Contains("403") || ex.Message.Contains("Unauthorized"))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Authentication Error", 
                    "Your session has expired. Please log in again.", 
                    "OK");
                await Shell.Current.GoToAsync("//");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"Unexpected error deleting customer: {ex.Message}", 
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}