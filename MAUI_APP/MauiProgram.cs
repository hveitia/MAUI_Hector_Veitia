using DevExpress.Maui;
using DevExpress.Maui.Core;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp.Views.Maui.Controls.Hosting;
using MAUI_APP.Services;
using MAUI_APP.ViewModels;
using MAUI_APP.Views;
using MAUI_APP.Views.Customers;

namespace MAUI_APP;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        ThemeManager.ApplyThemeToSystemBars = true;
        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UseDevExpress(useLocalization: false)
            .UseDevExpressControls()
            .UseDevExpressCharts()
            .UseDevExpressTreeView()
            .UseDevExpressCollectionView()
            .UseDevExpressEditors()
            .UseDevExpressDataGrid()
            .UseDevExpressScheduler()
            .UseDevExpressGauges()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("roboto-bold.ttf", "Roboto-Bold");
                fonts.AddFont("roboto-medium.ttf", "Roboto-Medium");
                fonts.AddFont("roboto-regular.ttf", "Roboto");
            });

        // Register HTTP Client como Singleton para mantener el token
        builder.Services.AddHttpClient("MainHttpClient", client =>
        {
            client.BaseAddress = new Uri("https://veitia.xari.net/api/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // Register HttpService como Singleton para compartir la misma instancia
        builder.Services.AddSingleton<IHttpService, HttpService>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("MainHttpClient");
            return new HttpService(httpClient);
        });

        // Register API Services
        builder.Services.AddScoped<IApiService, ApiService>();
        
        // Register ViewModels
        builder.Services.AddScoped<LoginViewModel>();
        builder.Services.AddScoped<CustomersViewModel>();
        
        // Register Views
        builder.Services.AddScoped<DefaultPage>();
        builder.Services.AddScoped<CustomersPage>();

        return builder.Build();
    }
}