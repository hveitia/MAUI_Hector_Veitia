# Aplicación MAUI - Gestión de Usuarios

## Descripción del Proyecto

Esta es una aplicación multiplataforma desarrollada con .NET MAUI (Multi-platform App UI) que implementa un sistema completo de gestión de usuarios. La aplicación está dirigida principalmente a la plataforma Android utilizando .NET 9.0 e integra los componentes de DevExpress MAUI para proporcionar una interfaz de usuario moderna y profesional.

## Características Principales

- **Arquitectura MVVM**: Implementación completa del patrón Model-View-ViewModel
- **Integración de API**: Conexión con servicios web para gestión de usuarios
- **Componentes DevExpress**: Suite completa de controles profesionales
- **Autenticación**: Sistema de login con gestión de tokens
- **Gestión de Usuarios**: CRUD completo (Crear, Leer, Actualizar, Eliminar)
- **Temas Adaptativos**: Sistema de temas que se adapta al dispositivo

## Tecnologías Utilizadas

### Framework Principal
- **.NET MAUI 9.0**: Framework multiplataforma de Microsoft
- **C#**: Lenguaje de programación principal
- **XAML**: Para el diseño de la interfaz de usuario

### Componentes y Librerías
- **DevExpress MAUI Suite 25.1**: Conjunto completo de controles empresariales
  - Controles básicos, gráficos, editores
  - DataGrid, TreeView, CollectionView
  - Programador, medidores y más
- **Microsoft.Extensions.Http**: Para la gestión de clientes HTTP
- **System.Text.Json**: Serialización JSON nativa

### Fuentes Tipográficas
- **OpenSans**: Fuente principal de la aplicación
- **Roboto**: Variantes para estilos específicos (Regular, Medium, Bold)

## Arquitectura del Proyecto

### Estructura de Directorios

```
MAUI_APP/
├── Models/                 # Modelos de datos y contratos de API
│   ├── ApiResponse.cs      # Wrapper genérico para respuestas de API
│   └── User.cs             # Modelos de usuario y DTOs de autenticación
├── Services/               # Capa de servicios con interfaces
│   ├── IHttpService.cs     # Interfaz para operaciones HTTP
│   ├── HttpService.cs      # Implementación de operaciones HTTP básicas
│   ├── IApiService.cs      # Interfaz para operaciones de API
│   └── ApiService.cs       # Implementación de operaciones de API de alto nivel
├── ViewModels/             # ViewModels del patrón MVVM
│   ├── BaseViewModel.cs    # Clase base con INotifyPropertyChanged
│   ├── LoginViewModel.cs   # Lógica de autenticación
│   └── UsersViewModel.cs   # Lógica de gestión de usuarios
├── Views/                  # Páginas XAML e interfaces
│   ├── AppShell.xaml       # Contenedor de navegación Shell
│   └── DefaultPage.xaml    # Página principal de contenido
├── Resources/              # Recursos de la aplicación
│   ├── Fonts/              # Familias de fuentes OpenSans y Roboto
│   ├── Styles/             # Colors.xaml y Styles.xaml con temas DevExpress
│   └── Images/             # Iconos e imágenes de la app
└── Platforms/              # Código específico de plataforma (Android/iOS)
```

### Patrón de Arquitectura de Servicios

La aplicación utiliza un enfoque de servicios en capas:

1. **HttpService** - Operaciones HTTP genéricas (GET, POST, PUT, DELETE)
2. **ApiService** - Llamadas específicas de la API utilizando HttpService
3. **ViewModels** - Consumen ApiService con manejo adecuado de errores

## Configuración y Desarrollo

### Comandos de Desarrollo

```bash
# Construir la solución completa
dotnet build MAUI_APP.sln

# Construir proyecto específico
dotnet build MAUI_APP/MAUI_APP.csproj

# Construir para plataforma Android
dotnet build MAUI_APP/MAUI_APP.csproj -f net9.0-android

# Restaurar paquetes NuGet
dotnet restore

# Agregar referencia de paquete
dotnet add MAUI_APP/MAUI_APP.csproj package [NombrePaquete]
```

### Configuración de API

La aplicación está configurada para conectarse a:
- **URL Base**: `https://veitia.xari.net/api/`
- **Autenticación**: Sistema basado en tokens JWT
- **Serialización**: JSON con manejo automático de errores

## Funcionalidades del Sistema

### Autenticación
- Login de usuarios con validación
- Gestión automática de tokens de autenticación
- Manejo de errores de autenticación

### Gestión de Usuarios
- Listar todos los usuarios
- Obtener detalles de usuario específico
- Crear nuevos usuarios
- Actualizar información de usuarios existentes
- Eliminar usuarios del sistema

### Sistema de Temas
- Temas adaptativos que siguen las preferencias del sistema
- Colores consistentes usando el sistema de temas DevExpress
- Soporte para modo claro y oscuro
- Estilos personalizados para navegación y botones

## Requisitos del Sistema

### Plataformas Soportadas
- **Android**: API nivel 21 o superior
- **iOS**: Estructura preparada para implementación futura

### Herramientas de Desarrollo
- **Visual Studio 2022** o **Visual Studio Code**
- **.NET 9.0 SDK**
- **Android SDK** (para desarrollo Android)
- **Emulador Android** o dispositivo físico para pruebas

## Instalación y Ejecución

1. **Clonar el repositorio**
   ```bash
   git clone [URL_DEL_REPOSITORIO]
   cd MAUI_APP
   ```

2. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

3. **Construir la aplicación**
   ```bash
   dotnet build
   ```

4. **Ejecutar en Android**
   - Abrir con Visual Studio
   - Seleccionar un emulador Android o dispositivo físico
   - Presionar F5 o hacer clic en "Ejecutar"

## Contribución

Este proyecto sigue las mejores prácticas de desarrollo .NET y MAUI:

- **Patrón MVVM**: Separación clara entre lógica de negocio y presentación
- **Inyección de Dependencias**: Configurada en `MauiProgram.cs`
- **Manejo de Errores**: Consistente a través de toda la aplicación
- **Temas**: Sistema unificado usando DevExpress Theme Manager

## Notas Técnicas

- La aplicación utiliza `System.Text.Json` para serialización JSON
- Los servicios HTTP están configurados con factory pattern
- El sistema de navegación usa Shell de MAUI
- Los estilos siguen las pautas de Material Design adaptadas con DevExpress

---

*Desarrollado con .NET MAUI y DevExpress Components*