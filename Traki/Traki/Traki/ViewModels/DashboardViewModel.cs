using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.ViewModels;
using Core.Views;
using TrakiLibrary.Interfaces;

namespace Traki.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        public DashboardViewModel(string dbPath, IAccountService accountService, IServiceProvider serviceProvider   )
        {
            _serviceProvider = serviceProvider;
        }
#pragma warning disable

        [ObservableProperty]
        private View? selectedTabView;

#pragma warning restore


        [RelayCommand]
        private void ShowIncome()
        {
            var viewModel = _serviceProvider.GetRequiredService<IncomeViewModel>();
            SelectedTabView = new IncomeView(viewModel);
        }

        [RelayCommand]
        private void ShowExpense()
        {
            SelectedTabView = new ExpenseView();
        }

        [RelayCommand]
        private void ShowTransactions()
        {
            SelectedTabView = new TransactionView();
        }
    }
}
