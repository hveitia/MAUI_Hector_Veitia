using DevExpress.Maui.Core;
using MAUI_APP.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MAUI_APP.Views;

public partial class DefaultPage : ContentPage
{
    private LoginViewModel? _viewModel;
    
    public DefaultPage()
    {
        InitializeComponent();
        var serviceProvider = Application.Current?.Handler?.MauiContext?.Services;
        if (serviceProvider != null)
        {
            _viewModel = serviceProvider.GetRequiredService<LoginViewModel>();
            BindingContext = _viewModel;
        }
    }

}