using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Views;
using TrakiLibrary.Interfaces;

namespace Traki.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {

        public DashboardViewModel(string dbPath, IAccountService accountService)
        {

        }
#pragma warning disable

        [ObservableProperty]
        private View? selectedTabView;

#pragma warning restore


        [RelayCommand]
        private void ShowIncome()
        {
           SelectedTabView = new IncomeView();
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
