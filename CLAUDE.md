# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Language Preference
**IMPORTANTE: Claude debe responder SIEMPRE en español cuando trabaje en este proyecto.** Todas las explicaciones, comentarios y comunicación deben ser en español, excepto cuando se trate de código o comandos específicos.

## Project Overview

This is a .NET MAUI (Multi-platform App UI) application targeting Android platforms using .NET 9.0. The project extensively uses DevExpress MAUI components and implements a complete MVVM architecture with API integration.

## Architecture

- **Entry Point**: `MauiProgram.cs` configures the MAUI app with DevExpress components, dependency injection, and HTTP services
- **MVVM Pattern**: Uses BaseViewModel with INotifyPropertyChanged, ViewModels for business logic, and dependency injection
- **API Architecture**: Layered HTTP service pattern with ApiService wrapping HttpService for type-safe API calls
- **Navigation**: Shell-based navigation with custom DevExpress toggle button styling
- **Target Platform**: Android (API 21+) with iOS structure in place

## Common Development Commands

### Build and Run
```bash
# Build the solution
dotnet build MAUI_APP.sln

# Build specific project
dotnet build MAUI_APP/MAUI_APP.csproj

# Build for Android platform
dotnet build MAUI_APP/MAUI_APP.csproj -f net9.0-android

# Restore NuGet packages
dotnet restore

# Add package reference
dotnet add MAUI_APP/MAUI_APP.csproj package [PackageName]
```

## Key Dependencies and Configuration

### Core Framework
- **Microsoft.Maui.Controls** (9.0.50) - Core MAUI framework
- **Microsoft.Extensions.Http** (9.0.7) - HTTP client factory support

### DevExpress Suite (25.1.*)
Complete DevExpress MAUI component suite configured in `MauiProgram.cs`:
- Controls, Charts, TreeView, CollectionView
- Editors, DataGrid, Scheduler, Gauges
- Theme manager with system bar theming enabled

### HTTP Configuration
Base API URL configured in `MauiProgram.cs`: `https://veitia.xari.net/api/`
- Registered with DI container using HttpClient factory pattern
- JSON serialization with proper error handling

## Project Structure

```
MAUI_APP/
├── Models/             # Data models and API contracts
│   ├── ApiResponse.cs  # Generic API response wrapper
│   └── User.cs         # User models and authentication DTOs
├── Services/           # Service layer with interfaces
│   ├── IHttpService.cs / HttpService.cs      # Low-level HTTP operations
│   └── IApiService.cs / ApiService.cs        # High-level API operations
├── ViewModels/         # MVVM ViewModels
│   ├── BaseViewModel.cs    # Base with INotifyPropertyChanged
│   ├── LoginViewModel.cs   # Authentication logic
│   └── UsersViewModel.cs   # User management logic
├── Views/              # XAML pages and code-behind
│   ├── AppShell.xaml   # Shell navigation container
│   └── DefaultPage.xaml # Main content page
├── Resources/          # App resources
│   ├── Fonts/          # OpenSans and Roboto font families
│   ├── Styles/         # Colors.xaml and Styles.xaml with DevExpress theming
│   └── Images/         # App icons and images
└── Platforms/          # Platform-specific code (Android/iOS)
```

## Service Architecture Pattern

The app uses a layered service approach:

1. **HttpService** (`IHttpService`) - Generic HTTP operations (GET, POST, PUT, DELETE)
2. **ApiService** (`IApiService`) - Business-specific API calls using HttpService
3. **ViewModels** - Consume ApiService with proper error handling

Key features:
- Automatic JSON serialization/deserialization
- Consistent error handling with ApiResponse wrapper
- Authentication token management
- Type-safe API contracts

## DevExpress Theming System

The app uses DevExpress theming throughout:

### Theme Colors
All styling uses `{dx:ThemeColor Key=ColorName}` bindings:
- Primary colors: Primary, OnPrimary, SecondaryContainer
- Surface colors: Surface, OnSurface, SurfaceContainer
- State colors: Outline, OnSurfaceVariant

### Custom Styling
- **Shell Navigation**: Custom DXToggleButton styling with rounded corners and theme-aware colors
- **Material Design**: Consistent button styling with proper states (Normal, Disabled, PointerOver)
- **Typography**: OpenSans as primary font, Roboto variants for specific styles

### Font Configuration
Configured in `MauiProgram.cs`:
- OpenSans-Regular.ttf → "OpenSansRegular"
- roboto-bold.ttf → "Roboto-Bold"
- roboto-medium.ttf → "Roboto-Medium"
- roboto-regular.ttf → "Roboto"

## API Integration Patterns

### Authentication Flow
LoginViewModel handles authentication with proper error handling and automatic token configuration.

### CRUD Operations
ApiService provides type-safe methods for user management:
- `GetUsersAsync()` - List users
- `GetUserAsync(int userId)` - Get single user
- `CreateUserAsync(User user)` - Create user
- `UpdateUserAsync(int userId, User user)` - Update user
- `DeleteUserAsync(int userId)` - Delete user

### Error Handling
Consistent error handling with ApiResponse<T> wrapper:
- Success/Error flags
- Typed data responses
- Localized error messages in Spanish

## Customer Management Implementation

### Customer Model
Located in `Models/User.cs`, the Customer class includes:
- **Oid**: Unique identifier from API
- **Name/LastName**: Customer names
- **Birthday**: Customer birth date
- **Active**: Status flag
- **FullName**: Computed property combining Name + LastName

### CustomersPage Implementation
- **Location**: `Views/Customers/CustomersPage.xaml`
- **ViewModel**: `ViewModels/CustomersViewModel.cs`
- **Navigation**: Registered in AppShell with route "customers"
- **Auto-loading**: Customers load automatically when page appears
- **UI Components**: Uses DXCollectionView with card-style customer display

### Navigation Flow
After successful login:
1. LoginViewModel navigates to "//customers" route
2. CustomersPage loads with bound CustomersViewModel
3. Page automatically calls GetCustomersAsync() on appearance
4. Customers display in themed card layout with DevExpress styling

## Authentication Token Management

### Issue Resolved: 403 Unauthorized Error
**Problem**: HttpService instances were not sharing the authentication token due to dependency injection configuration.

**Root Cause**: Using `AddHttpClient<IHttpService, HttpService>()` created separate instances, causing the token set in LoginViewModel to be lost when CustomersViewModel was instantiated.

**Solution**: 
- Changed to **Singleton pattern** for HttpService in `MauiProgram.cs`
- Uses named HttpClient factory pattern to maintain single instance
- Token persists across all service calls throughout app lifecycle

### Implementation Details
```csharp
// HttpService registered as Singleton
builder.Services.AddSingleton<IHttpService, HttpService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("MainHttpClient");
    return new HttpService(httpClient);
});
```

### Debug Features Added
- Comprehensive HTTP request/response logging
- Token verification methods (`HasAuthToken()`)
- Authentication error detection and user notifications
- Automatic re-login flow for expired tokens

## JSON Deserialization Issue Resolved

### Issue: JSON Conversion Error
**Problem**: `The JSON value could not be converted to System.String` error when calling GetCustomersAsync.

**Root Cause**: HttpService was attempting double deserialization:
1. First deserializing JSON response to `string`
2. Then deserializing that string to target type `T`

This fails when the API returns direct JSON arrays/objects instead of JSON-encoded strings.

**Solution**: Implemented fallback deserialization strategy:
1. **Try direct deserialization first** (most common case)
2. **Fallback to double deserialization** if direct fails (for APIs that return JSON as strings)
3. **Comprehensive error logging** to identify which approach succeeded

### Implementation
```csharp
// Try direct deserialization first
try
{
    var result = JsonSerializer.Deserialize<T>(content);
    return result;
}
catch (JsonException)
{
    // Fallback to double deserialization
    string innerJson = JsonSerializer.Deserialize<string>(content);
    var result = JsonSerializer.Deserialize<T>(innerJson);
    return result;
}
```

## OData Integration Support

### Issue: OData Response Structure
**Problem**: API returns OData format with `@odata.context` and `value` wrapper, but code expected direct array.

**Example API Response**:
```json
{
  "@odata.context": "https://veitia.xari.net/api/odata/$metadata#Customer",
  "value": [
    {
      "Oid": "6c7baa43-c489-42ac-b7d7-aab420ed111d",
      "Birthday": "0001-01-01T00:00:00Z",
      "Name": "Hector",
      "LastName": "Veitia",
      "Active": true
    }
  ]
}
```

**Solution**: Created `ODataResponse<T>` generic model to handle OData wrapper structure.

### Implementation
```csharp
public class ODataResponse<T>
{
    [JsonPropertyName("@odata.context")]
    public string? Context { get; set; }

    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = [];
}
```

### Usage in ApiService
```csharp
var odataResponse = await _httpService.GetAsync<ODataResponse<Customer>>("odata/Customer");
return odataResponse?.Value ?? [];
```