# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

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