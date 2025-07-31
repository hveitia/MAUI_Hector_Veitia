using MAUI_APP.Services;
using MAUI_APP.ViewModels;
using MAUI_APP.Models;
using System.Windows.Input;

public class CustomersViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IHttpService _httpService;

    private List<Customer> _allCustomers  = [];
    private Customer _editingCustomer = new Customer();
    private bool _isEditPopupOpen = false;
    private DateTime _editingCustomerBirthday = DateTime.Today;
    
    public CustomersViewModel(IApiService apiService, IHttpService httpService)
    {
        _apiService = apiService;
        _httpService = httpService;
        GetCustomersCommand = new Command(async () => await GetCustomersAsync(), () => !IsBusy);
        DeleteCustomerCommand = new Command<string>(async (oid) => await DeleteCustomerAsync(oid), (oid) => !IsBusy && !string.IsNullOrEmpty(oid));
        EditCustomerCommand = new Command<Customer>((customer) => OpenEditPopup(customer), (customer) => !IsBusy && customer != null);
        SaveCustomerCommand = new Command(async () => await SaveCustomerAsync(), () => !IsBusy);
        CloseEditPopupCommand = new Command(() => CloseEditPopup());
        Title = "Customers";
    }

    public ICommand GetCustomersCommand { get; }

    public ICommand DeleteCustomerCommand { get; }
    
    public ICommand EditCustomerCommand { get; }
    
    public ICommand SaveCustomerCommand { get; }
    
    public ICommand CloseEditPopupCommand { get; }

    public List<Customer> AllCustomers
    {
        get => _allCustomers;
        set => SetProperty(ref _allCustomers, value);
    }
    
    public Customer EditingCustomer
    {
        get => _editingCustomer;
        set => SetProperty(ref _editingCustomer, value);
    }
    
    public bool IsEditPopupOpen
    {
        get => _isEditPopupOpen;
        set => SetProperty(ref _isEditPopupOpen, value);
    }
    
    public DateTime EditingCustomerBirthday
    {
        get => _editingCustomerBirthday;
        set => SetProperty(ref _editingCustomerBirthday, value);
    }
    
    public async Task GetCustomersAsync()
    {
        IsBusy = true;
        try
        {
            System.Diagnostics.Debug.WriteLine("CustomersViewModel: Starting customer loading...");
            
            // Check if we have authentication token
            if (!_httpService.HasAuthToken())
            {
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: NO AUTHENTICATION TOKEN");
                await Shell.Current.DisplayAlert(
                    "Authentication Error", 
                    "You don't have permission to access this information. Please log in again.", 
                    "OK");
                    
                // Navigate back to login
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
            
            // Check if it's an authentication error
            if (ex.Message.Contains("401") || ex.Message.Contains("403") || ex.Message.Contains("Unauthorized"))
            {
                await Shell.Current.DisplayAlert(
                    "Authentication Error", 
                    "Your session has expired. Please log in again.", 
                    "OK");
                await Shell.Current.GoToAsync("//");
            }
            else
            {
                await Shell.Current.DisplayAlert(
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
            await Shell.Current.DisplayAlert(
                "Error", 
                "Invalid customer ID.", 
                "OK");
            return;
        }

        IsBusy = true;
        try
        {
            System.Diagnostics.Debug.WriteLine($"CustomersViewModel: Starting customer deletion with OID: {oid}");
            
            // Find customer in local list to show their name in confirmation
            var customerToDelete = AllCustomers.FirstOrDefault(c => c.Oid == oid);
            string customerName = customerToDelete?.FullName ?? "Unknown customer";
            
            // Show confirmation to user
            bool confirm = await Shell.Current.DisplayAlert(
                "Confirm Deletion",
                $"Are you sure you want to delete {customerName}? This action cannot be undone.",
                "Delete",
                "Cancel");
                
            if (!confirm)
            {
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: Deletion cancelled by user");
                return;
            }
            
            // Check if we have authentication token
            if (!_httpService.HasAuthToken())
            {
                System.Diagnostics.Debug.WriteLine("CustomersViewModel: NO AUTHENTICATION TOKEN for deletion");
                await Shell.Current.DisplayAlert(
                    "Authentication Error", 
                    "You don't have permission to perform this action. Please log in again.", 
                    "OK");
                    
                await Shell.Current.GoToAsync("//");
                return;
            }
            
            // Call API service to delete customer
            var result = await _apiService.DeleteCustomerAsync(oid);
            
            if (result != null && result is bool success && success)
            {
                System.Diagnostics.Debug.WriteLine($"CustomersViewModel: Customer deleted successfully: {oid}");
                
                // Remove customer from local list
                if (customerToDelete != null)
                {
                    var updatedList = AllCustomers.ToList();
                    updatedList.Remove(customerToDelete);
                    AllCustomers = updatedList;
                }
                
                await Shell.Current.DisplayAlert(
                    "Success", 
                    $"{customerName} has been deleted successfully.", 
                    "OK");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"CustomersViewModel: Error deleting customer");
                await Shell.Current.DisplayAlert(
                    "Error", 
                    $"Could not delete customer ", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CustomersViewModel Deletion error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            
            // Check if it's an authentication error
            if (ex.Message.Contains("401") || ex.Message.Contains("403") || ex.Message.Contains("Unauthorized"))
            {
                await Shell.Current.DisplayAlert(
                    "Authentication Error", 
                    "Your session has expired. Please log in again.", 
                    "OK");
                await Shell.Current.GoToAsync("//");
            }
            else
            {
                await Shell.Current.DisplayAlert(
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
    
    public void OpenEditPopup(Customer customer)
    {
        if (customer == null) return;
        
        System.Diagnostics.Debug.WriteLine($"Opening edit popup for customer: {customer.FullName}");
        
        // Create a copy of the customer for editing
        EditingCustomer = new Customer
        {
            Oid = customer.Oid,
            Name = customer.Name,
            LastName = customer.LastName,
            Birthday = customer.Birthday,
            Active = customer.Active
        };
        
        // Convert date string to DateTime for DatePicker
        if (DateTime.TryParse(customer.Birthday, out DateTime parsedDate))
        {
            EditingCustomerBirthday = parsedDate;
        }
        else
        {
            EditingCustomerBirthday = DateTime.Today;
        }
        
        IsEditPopupOpen = true;
    }
    
    public void CloseEditPopup()
    {
        IsEditPopupOpen = false;
        EditingCustomer = new Customer();
        EditingCustomerBirthday = DateTime.Today;
    }
    
    public async Task SaveCustomerAsync()
    {
        // Input validation
        if (EditingCustomer == null || string.IsNullOrEmpty(EditingCustomer.Oid))
        {
            await Shell.Current.DisplayAlert(
                "Validation Error", 
                "No customer data available to save.", 
                "OK");
            return;
        }
        
        // Validate required fields
        var validationErrors = ValidateCustomerData();
        if (validationErrors.Any())
        {
            await Shell.Current.DisplayAlert(
                "Validation Error", 
                string.Join("\n", validationErrors), 
                "OK");
            return;
        }

        IsBusy = true;
        try
        {
            System.Diagnostics.Debug.WriteLine($"Saving customer: {EditingCustomer.FullName}");
            
            // Prepare customer data for update
            var customerToUpdate = PrepareCustomerForUpdate();
            
            // Check authentication
            if (!_httpService.HasAuthToken())
            {
                System.Diagnostics.Debug.WriteLine("CustomersViewModel SaveCustomer: NO AUTHENTICATION TOKEN");
                await ShowAuthenticationError();
                return;
            }
            
            // Call API to update customer
            var result = await _apiService.UpdateCustomerAsync(EditingCustomer.Oid, customerToUpdate);
            
            if (IsUpdateSuccessful(result))
            {
                System.Diagnostics.Debug.WriteLine($"Customer updated successfully: {EditingCustomer.Oid}");
                
                // Update local customer list
                UpdateLocalCustomerList(customerToUpdate);
                
                await Shell.Current.DisplayAlert(
                    "Success", 
                    $"Customer {EditingCustomer.FullName} has been updated successfully.", 
                    "OK");
                    
                CloseEditPopup();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error updating customer");
                await Shell.Current.DisplayAlert(
                    "Update Failed", 
                    "Could not update customer. Please try again.", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CustomersViewModel SaveCustomer error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            
            await HandleSaveException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private List<string> ValidateCustomerData()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(EditingCustomer.Name))
            errors.Add("Customer name is required.");
            
        if (string.IsNullOrWhiteSpace(EditingCustomer.LastName))
            errors.Add("Customer last name is required.");
            
        if (EditingCustomerBirthday == default)
            errors.Add("Please select a valid birth date.");
            
        if (EditingCustomerBirthday > DateTime.Today)
            errors.Add("Birth date cannot be in the future.");
            
        return errors;
    }

    private Customer PrepareCustomerForUpdate()
    {
        return new Customer
        {
            Oid = EditingCustomer.Oid,
            Name = EditingCustomer.Name.Trim(),
            LastName = EditingCustomer.LastName.Trim(),
            Birthday = EditingCustomerBirthday.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Active = EditingCustomer.Active
        };
    }

    private async Task ShowAuthenticationError()
    {
        await Shell.Current.DisplayAlert(
            "Authentication Error", 
            "You don't have permission to perform this action. Please log in again.", 
            "OK");
        await Shell.Current.GoToAsync("//");
    }

    private static bool IsUpdateSuccessful(object result)
    {
        return result != null && result is not bool;
    }

    private void UpdateLocalCustomerList(Customer updatedCustomer)
    {
        var customerIndex = AllCustomers.FindIndex(c => c.Oid == updatedCustomer.Oid);
        if (customerIndex >= 0)
        {
            var updatedList = AllCustomers.ToList();
            updatedList[customerIndex] = updatedCustomer;
            AllCustomers = updatedList;
        }
    }

    private async Task HandleSaveException(Exception ex)
    {
        // Check if it's an authentication error
        if (ex.Message.Contains("401") || ex.Message.Contains("403") || ex.Message.Contains("Unauthorized"))
        {
            await Shell.Current.DisplayAlert(
                "Authentication Error", 
                "Your session has expired. Please log in again.", 
                "OK");
            await Shell.Current.GoToAsync("//");
        }
        else
        {
            await Shell.Current.DisplayAlert(
                "Unexpected Error", 
                $"An unexpected error occurred while updating the customer: {ex.Message}", 
                "OK");
        }
    }
}