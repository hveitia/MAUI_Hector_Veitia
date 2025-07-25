# API Usage Guide - MAUI_APP

Este README explica cómo usar la estructura de API implementada en la aplicación .NET MAUI.

## 📋 Índice

- [Configuración Inicial](#configuración-inicial)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Ejemplos de Uso](#ejemplos-de-uso)
- [ViewModels](#viewmodels)
- [Binding en XAML](#binding-en-xaml)
- [Extensión de la API](#extensión-de-la-api)
- [Troubleshooting](#troubleshooting)

## 🛠️ Configuración Inicial

### 1. Configurar la URL Base de la API

En `MauiProgram.cs`, actualiza la URL base de tu API:

```csharp
builder.Services.AddHttpClient<IHttpService, HttpService>(client =>
{
    client.BaseAddress = new Uri("https://tu-api.com/api/"); // Cambia esta URL
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
```

### 2. Registrar ViewModels (Opcional)

Si quieres usar inyección de dependencias para ViewModels:

```csharp
// En MauiProgram.cs, después de registrar los servicios
builder.Services.AddTransient<LoginViewModel>();
builder.Services.AddTransient<UsersViewModel>();
```

## 🏗️ Estructura del Proyecto

```
MAUI_APP/
├── Models/
│   ├── ApiResponse.cs          # Respuesta genérica de la API
│   └── User.cs                 # Modelos de usuario y login
├── Services/
│   ├── IHttpService.cs         # Interfaz HTTP
│   ├── HttpService.cs          # Implementación HTTP
│   ├── IApiService.cs          # Interfaz de servicios API
│   └── ApiService.cs           # Implementación de servicios API
└── ViewModels/
    ├── BaseViewModel.cs        # ViewModel base
    ├── LoginViewModel.cs       # ViewModel de login
    └── UsersViewModel.cs       # ViewModel de usuarios
```

## 🚀 Ejemplos de Uso

### 1. Usar el HttpService Directamente

```csharp
public class MiViewModel : BaseViewModel
{
    private readonly IHttpService _httpService;
    
    public MiViewModel(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task EjemploGetAsync()
    {
        // GET request
        var usuarios = await _httpService.GetAsync<List<User>>("users");
        
        // POST request
        var nuevoUsuario = new User { Username = "john", Email = "john@example.com" };
        var resultado = await _httpService.PostAsync<User>("users", nuevoUsuario);
        
        // PUT request
        var usuarioActualizado = await _httpService.PutAsync<User>("users/1", nuevoUsuario);
        
        // DELETE request
        var eliminado = await _httpService.DeleteAsync<ApiResponse>("users/1");
    }
}
```

### 2. Usar el ApiService (Recomendado)

```csharp
public class MiViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    
    public MiViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    public async Task EjemploLoginAsync()
    {
        var loginRequest = new LoginRequest
        {
            Username = "usuario",
            Password = "contraseña"
        };
        
        var response = await _apiService.LoginAsync(loginRequest);
        
        if (response.Success && response.Data != null)
        {
            // Login exitoso
            var token = response.Data.Token;
            var usuario = response.Data.User;
            
            // El token se configura automáticamente en HttpService
        }
        else
        {
            // Manejar error
            var error = response.Error ?? "Error desconocido";
        }
    }
}
```

## 📱 ViewModels

### LoginViewModel - Ejemplo de Uso

```csharp
// En tu página/view
public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;
    
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    
    // O si no usas inyección de dependencias:
    public LoginPage()
    {
        InitializeComponent();
        var apiService = Handler.MauiContext.Services.GetService<IApiService>();
        var httpService = Handler.MauiContext.Services.GetService<IHttpService>();
        BindingContext = new LoginViewModel(apiService, httpService);
    }
}
```

### UsersViewModel - Ejemplo de Uso

```csharp
public partial class UsersPage : ContentPage
{
    private readonly UsersViewModel _viewModel;
    
    public UsersPage(UsersViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUsersAsync();
    }
}
```

## 🎨 Binding en XAML

### Ejemplo de Login Page

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage x:Class="MAUI_APP.Views.LoginPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Title="{Binding Title}">
    
    <ScrollView>
        <VerticalStackLayout Padding="30" Spacing="25">
            
            <Label Text="Iniciar Sesión" 
                   FontSize="24" 
                   HorizontalOptions="Center" />
            
            <Entry Text="{Binding Username}" 
                   Placeholder="Usuario" />
            
            <Entry Text="{Binding Password}" 
                   Placeholder="Contraseña" 
                   IsPassword="True" />
            
            <Label Text="{Binding ErrorMessage}" 
                   TextColor="Red" 
                   IsVisible="{Binding ErrorMessage, Converter={StaticResource StringToBoolConverter}}" />
            
            <Button Text="Iniciar Sesión" 
                    Command="{Binding LoginCommand}" 
                    IsEnabled="{Binding IsBusy, Converter={StaticResource InvertedBoolConverter}}" />
            
            <ActivityIndicator IsRunning="{Binding IsBusy}" 
                              IsVisible="{Binding IsBusy}" />
            
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

### Ejemplo de Users Page con DevExpress DataGrid

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage x:Class="MAUI_APP.Views.UsersPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:dx="http://schemas.devexpress.com/maui"
             Title="{Binding Title}">
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>
        
        <!-- Toolbar -->
        <StackLayout Grid.Row="0" 
                     Orientation="Horizontal" 
                     Padding="10">
            <Button Text="Actualizar" 
                    Command="{Binding RefreshCommand}" />
            <ActivityIndicator IsRunning="{Binding IsBusy}" 
                              IsVisible="{Binding IsBusy}" />
        </StackLayout>
        
        <!-- DataGrid -->
        <dx:DXDataGrid Grid.Row="1" 
                       ItemsSource="{Binding Users}"
                       SelectedItem="{Binding SelectedUser}">
            <dx:DXDataGrid.Columns>
                <dx:TextColumn FieldName="Username" Caption="Usuario" />
                <dx:TextColumn FieldName="Email" Caption="Email" />
                <dx:TextColumn FieldName="FirstName" Caption="Nombre" />
                <dx:TextColumn FieldName="LastName" Caption="Apellido" />
                <dx:CheckBoxColumn FieldName="IsActive" Caption="Activo" />
            </dx:DXDataGrid.Columns>
        </dx:DXDataGrid>
        
    </Grid>
</ContentPage>
```

## 🔧 Extensión de la API

### Agregar un Nuevo Endpoint

1. **Crear el modelo** (si es necesario):

```csharp
// En Models/Product.cs
public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}
```

2. **Extender IApiService**:

```csharp
// En Services/IApiService.cs
public interface IApiService
{
    // Métodos existentes...
    
    // Nuevos métodos para productos
    Task<ApiResponse<List<Product>>> GetProductsAsync();
    Task<ApiResponse<Product>> GetProductAsync(int productId);
    Task<ApiResponse<Product>> CreateProductAsync(Product product);
}
```

3. **Implementar en ApiService**:

```csharp
// En Services/ApiService.cs
public async Task<ApiResponse<List<Product>>> GetProductsAsync()
{
    var response = await _httpService.GetAsync<ApiResponse<List<Product>>>("products");
    return response ?? new ApiResponse<List<Product>> 
    { 
        Success = false, 
        Error = "Error al obtener productos" 
    };
}

public async Task<ApiResponse<Product>> GetProductAsync(int productId)
{
    var response = await _httpService.GetAsync<ApiResponse<Product>>($"products/{productId}");
    return response ?? new ApiResponse<Product> 
    { 
        Success = false, 
        Error = "Error al obtener el producto" 
    };
}

public async Task<ApiResponse<Product>> CreateProductAsync(Product product)
{
    var response = await _httpService.PostAsync<ApiResponse<Product>>("products", product);
    return response ?? new ApiResponse<Product> 
    { 
        Success = false, 
        Error = "Error al crear el producto" 
    };
}
```

4. **Crear ViewModel**:

```csharp
// En ViewModels/ProductsViewModel.cs
public class ProductsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    
    public ProductsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Products = new ObservableCollection<Product>();
        LoadProductsCommand = new Command(async () => await LoadProductsAsync());
    }
    
    public ObservableCollection<Product> Products { get; }
    public ICommand LoadProductsCommand { get; }
    
    private async Task LoadProductsAsync()
    {
        IsBusy = true;
        
        try
        {
            var response = await _apiService.GetProductsAsync();
            
            if (response.Success && response.Data != null)
            {
                Products.Clear();
                foreach (var product in response.Data)
                {
                    Products.Add(product);
                }
            }
        }
        catch (Exception ex)
        {
            // Manejar error
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

### Configurar Headers Personalizados

```csharp
// En MauiProgram.cs
builder.Services.AddHttpClient<IHttpService, HttpService>(client =>
{
    client.BaseAddress = new Uri("https://tu-api.com/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", "tu-api-key");
    client.DefaultRequestHeaders.Add("User-Agent", "MAUI-App/1.0");
});
```

### Configurar Timeout

```csharp
// En MauiProgram.cs
builder.Services.AddHttpClient<IHttpService, HttpService>(client =>
{
    client.BaseAddress = new Uri("https://tu-api.com/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

## 🐛 Troubleshooting

### Error: "No se puede resolver el servicio"

```csharp
// Asegúrate de que los servicios estén registrados en MauiProgram.cs
builder.Services.AddHttpClient<IHttpService, HttpService>();
builder.Services.AddScoped<IApiService, ApiService>();
```

### Error: "BaseAddress no configurada"

```csharp
// Configura la BaseAddress en el HttpClient
client.BaseAddress = new Uri("https://tu-api.com/api/");
```

### Error de Autenticación

```csharp
// Verifica que el token se esté configurando correctamente
public async Task LoginAsync()
{
    var response = await _apiService.LoginAsync(loginRequest);
    if (response.Success && response.Data != null)
    {
        // El token se configura automáticamente en HttpService
        _httpService.SetAuthToken(response.Data.Token);
    }
}
```

### Error de Serialización JSON

```csharp
// Verifica que las propiedades tengan JsonPropertyName
public class User
{
    [JsonPropertyName("id")]  // Importante: coincide con la respuesta de la API
    public int Id { get; set; }
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}
```

### Error de Conectividad

```csharp
// Verifica permisos de red en AndroidManifest.xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

## 📚 Próximos Pasos

1. **Implementar caché local** con SQLite
2. **Agregar retry policies** para requests fallidos
3. **Implementar refresh tokens** para autenticación
4. **Agregar logging** más detallado
5. **Implementar offline support**

---

¿Necesitas ayuda con algún aspecto específico de la implementación? ¡Pregúntame!