using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Pages;
using Core.Shared;
using Core.ViewModels;
using Core.Views;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Traki.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, IRecipient<AccountChangedMessage>   
    {
        #region Private Variables
        private readonly IServiceProvider _serviceProvider;
        
#pragma warning disable
        [ObservableProperty]
        private View? selectedTabView;
        [ObservableProperty]
        public decimal balance;
#pragma warning restore
        ExpenseView expenseView;
        #endregion Private Variables

        #region Constructor
        public DashboardViewModel(IAccountService accountService, IServiceProvider serviceProvider   )
        {
            _serviceProvider = serviceProvider;
            // Set default tab
            ShowIncome();
            //StrongReferenceMessenger.Default.Register(this);
            StrongReferenceMessenger.Default.Register <AccountChangedMessage>(this);
        }
        #endregion Constructor

        #region Public Methods
        public void Receive(AccountChangedMessage accountChangedMessage)
        {
            var accountDetails = accountChangedMessage.Value;
            CalculateBalances(accountDetails);
        }

        #endregion Public Methods

        #region Commands
        [RelayCommand]
        private void ShowIncome()
        {
            var viewModel = _serviceProvider.GetRequiredService<IncomeViewModel>();
            SelectedTabView = new IncomeView(viewModel);

            // Initialize the expense view model.
            var expenseViewModel = _serviceProvider.GetRequiredService<ExpenseViewModel>();
            expenseView = new ExpenseView(expenseViewModel);
        }

        [RelayCommand]
        private void ShowExpense()
        {
            SelectedTabView = expenseView;
        }

        [RelayCommand]
        private void ShowTransactions()
        {
            SelectedTabView = new TransactionView();
        }

        [RelayCommand]
        private void UploadExcel()
        {
            Shell.Current.GoToAsync($"{nameof(ExcelUploaderPage)}?type=Expense");
        }

        [RelayCommand]
        private void DownloadExcel()
        {

        }

        [RelayCommand]
        private void OnAddMoneyClicked()
        {
            // Pass parameters as query parameters in the URL
            Shell.Current.GoToAsync($"{nameof(TransactionPage)}?type=Income");

        }
        [RelayCommand]
        private void OnWidthDrawMoneyClicked()
        {
            Shell.Current.GoToAsync($"{nameof(TransactionPage)}?type=Expense");
        }
        #endregion Commands

        #region Private Methods
        private void CalculateBalances(TransactionFilterRequest accountDetails)
        {
            var _transactions = accountDetails.Transactions;
            if (_transactions != null)
            {
                var totalIncome = _transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
                var totalExpenses = _transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);

                this.Balance = totalIncome + totalExpenses;
            }
        }
        #endregion Private Methods
    }
}
