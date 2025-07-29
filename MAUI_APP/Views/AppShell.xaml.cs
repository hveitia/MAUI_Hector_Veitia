using MAUI_APP.Views.Customers;

namespace MAUI_APP.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute("customers", typeof(CustomersPage));
    }
}