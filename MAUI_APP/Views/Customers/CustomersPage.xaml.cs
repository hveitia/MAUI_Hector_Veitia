using DevExpress.Maui.Core;
using MAUI_APP.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MAUI_APP.Views.Customers;

public partial class CustomersPage : ContentPage
{
    public CustomersPage(CustomersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is CustomersViewModel viewModel)
        {
            await viewModel.GetCustomersAsync();
        }
    }
}