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
        DeleteUserCommand = new Command<User>(async (user) => await DeleteUserAsync(user), (user) => !IsBusy);
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
    public ICommand DeleteUserCommand { get; }

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

    private async Task DeleteUserAsync(User user)
    {
        if (user == null) return;

        var result = await Shell.Current.DisplayAlert("Confirmar", $"¿Eliminar usuario {user.Username}?", "Sí", "No");
        if (!result) return;

        IsBusy = true;

        try
        {
            var response = await _apiService.DeleteUserAsync(user.Id);

            if (response.Success)
            {
                Users.Remove(user);
                await Shell.Current.DisplayAlert("Éxito", "Usuario eliminado correctamente", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", response.Error ?? "Error al eliminar usuario", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Error de conexión", "OK");
            System.Diagnostics.Debug.WriteLine($"Delete user error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}