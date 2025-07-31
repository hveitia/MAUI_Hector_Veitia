using System.Collections.ObjectModel;
using System.Windows.Input;
using MAUI_APP.Models;
using MAUI_APP.Services;

namespace MAUI_APP.ViewModels;

public class UsersViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private User? _selectedUser;

    public UsersViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Users = new ObservableCollection<User>();
        LoadUsersCommand = new Command(async () => await LoadUsersAsync(), () => !IsBusy);
        RefreshCommand = new Command(async () => await RefreshAsync(), () => !IsBusy);
        Title = "Usuarios";
    }

    public ObservableCollection<User> Users { get; }

    public User? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    public ICommand LoadUsersCommand { get; }
    public ICommand RefreshCommand { get; }

    public async Task LoadUsersAsync()
    {
        IsBusy = true;

        try
        {
            var response = await _apiService.GetUsersAsync();

            if (response.Success && response.Data != null)
            {
                Users.Clear();
                foreach (var user in response.Data)
                {
                    Users.Add(user);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", response.Error ?? "Error al cargar usuarios", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Error de conexión", "OK");
            System.Diagnostics.Debug.WriteLine($"Load users error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshAsync()
    {
        await LoadUsersAsync();
    }

    
}