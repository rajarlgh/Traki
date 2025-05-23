using Core.Pages;
using Traki.Pages;

namespace Traki
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ManageAccountsPage), typeof(ManageAccountsPage));
            Routing.RegisterRoute(nameof(ManageCategoriesPage), typeof(ManageCategoriesPage));
            Routing.RegisterRoute(nameof(ExcelUploaderPage), typeof(ExcelUploaderPage));
            Routing.RegisterRoute(nameof(TransactionPage), typeof(TransactionPage));
        }
    }
}
