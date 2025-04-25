using Core.Pages;
using Core.Views;

namespace Traki
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ManageAccountsPage), typeof(ManageAccountsPage));
        }
    }
}
