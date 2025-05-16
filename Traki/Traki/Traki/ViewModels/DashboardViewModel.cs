using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Enum;
using Core.Pages;
using Core.Shared;
using Core.Shared.Messages.Transactions;
using Core.ViewModels;
using Core.Views;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public DashboardViewModel(IAccountService accountService, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // Set default tab
            ShowIncome();
            //StrongReferenceMessenger.Default.Register(this);
            StrongReferenceMessenger.Default.Register<AccountChangedMessage>(this);

            WeakReferenceMessenger.Default.Register<SelectedTransactionMessage>(this, (r, m) =>
            {
                var selectedItem = m.Value;
                if (selectedItem != null)
                {
                    ShowTransactions(selectedItem);
                }
            });

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
        private void ShowTransactions(TransactionMessage transactionMessage)
        {
            var viewModel = _serviceProvider.GetService<DetailedTransactionsViewModel>();
            if (viewModel != null)
            {
                viewModel.ShowBreakdownForCategory(transactionMessage.CategoryId, transactionMessage.TransactionType);
                SelectedTabView = new DetailedTransactionsView(viewModel);
            }
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
            Chart c  = new Chart();
            var (transactionsByCategorys, transactionsByAccounts) = c.FilterTransactions(accountDetails);

            var selectedAccountId = accountDetails.AccountId;
            var transactionCategory = transactionsByCategorys;
            var transactionByAccounts = transactionsByAccounts;

            if (transactionCategory != null)
            {
                var incomeOfTransactionCategory = transactionCategory
                    .Where(t => t.Type == TransactionType.Income.ToString())
                    .Sum(t => t.Amount);

                var expenseOfTransactionCategory = transactionCategory
                    .Where(t => t.Type == TransactionType.Expense.ToString())
                    .Sum(t => t.Amount);

                decimal incomeOfTransactionByAccount = 0;
                decimal expenseOfTransactionByAccount = 0;

                if (transactionByAccounts != null)
                {
                    incomeOfTransactionByAccount = transactionByAccounts
                        .Where(t => t.Type == TransactionType.Income.ToString() && t.DestinationAccountId == selectedAccountId)
                        .Sum(t => t.Amount);

                    expenseOfTransactionByAccount = transactionByAccounts
                        .Where(t => t.Type == TransactionType.Expense.ToString())
                        .Sum(t => t.Amount);
                }

                this.Balance = incomeOfTransactionCategory + incomeOfTransactionByAccount +
                               expenseOfTransactionCategory + expenseOfTransactionByAccount;
            }
        }

        #endregion Private Methods
    }
}
