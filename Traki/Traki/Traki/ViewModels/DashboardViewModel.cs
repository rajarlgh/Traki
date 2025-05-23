using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Enum;
using Core.Pages;
using Core.Shared;
using Core.Shared.Messages.Dashboard;
using Core.Shared.Messages.Transactions;
using Core.ViewModels;
using Core.Views;
using System.Collections.ObjectModel;
using TrakiLibrary.Interfaces;

namespace Traki.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, IRecipient<AccountChangedMessage>
    {
        #region Private Variables
        private readonly IServiceProvider _serviceProvider;
        private readonly IBudgetContextService _budgetContextService;
        private bool _isInitializing;
#pragma warning disable
        [ObservableProperty]
        private View? selectedTabView;
        public ObservableCollection<string> AvailableBudgets { get; } = new();
        [ObservableProperty]
        public decimal balance;
#pragma warning restore
        ExpenseView expenseView;
        #endregion Private Variables

        #region Constructor
        public DashboardViewModel(IAccountService accountService, IServiceProvider serviceProvider, IBudgetContextService budgetContextServicer)
        {
            _serviceProvider = serviceProvider;
            _budgetContextService = budgetContextServicer;
            // Set default tab
            ShowIncome();
            //WeakReferenceMessenger.Default.Register(this);
            WeakReferenceMessenger.Default.Register<AccountChangedMessage>(this);

            WeakReferenceMessenger.Default.Register<SelectedTransactionMessage>(this, (r, m) =>
            {
                var selectedItem = m.Value;
                if (selectedItem != null)
                {
                    ShowTransactions(selectedItem);
                }
            });

            _budgetContextService.BudgetChanged += OnBudgetChanged;

            LoadAvailableBudgets();

            // Optional: Set default budget
            if (AvailableBudgets.Any())
            {
                SelectedBudget = AvailableBudgets.First();
                _isInitializing = false;
            }

        }

        #endregion Constructor

        #region Public Methods
        private void PublishFilterChanged(bool flag)
        {
            WeakReferenceMessenger.Default.Send(new BudgetChangedMessage());
            //WeakReferenceMessenger.Default.Send(new AccountChangedMessage(( new TransactionFilterRequest() )));
            //WeakReferenceMessenger.Default.Send(new FilterChangedMessage((new FilterState())));

        }
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
            Chart c = new Chart();

            // Step 1: Get balances from previous transactions (before FromDate)
            var (carryForwardCategories, carryForwardAccounts) = c.FilterTransactions(accountDetails, null, isForCarryForward: true);
            decimal carryForwardBalance = 0;

            if (carryForwardCategories != null)
            {
                var t1 = carryForwardCategories
                    .Where(t => t.Type == TransactionType.Income.ToString());
                carryForwardBalance += carryForwardCategories
                    .Where(t => t.Type == TransactionType.Income.ToString())
                    .Sum(t => t.Amount);

                var t2 = carryForwardCategories
                    .Where(t => t.Type == TransactionType.Expense.ToString());

                carryForwardBalance += carryForwardCategories
                    .Where(t => t.Type == TransactionType.Expense.ToString())
                    .Sum(t => t.Amount);
            }

            if (carryForwardAccounts != null)
            {
                var selectedAccountId = accountDetails.AccountId;
                var t1
                    = carryForwardAccounts
                    .Where(t => t.Type == TransactionType.Income.ToString() && t.DestinationAccountId == selectedAccountId);


                carryForwardBalance += carryForwardAccounts
                    .Where(t => t.Type == TransactionType.Income.ToString() && t.DestinationAccountId == selectedAccountId)
                    .Sum(t => t.Amount);

                var t2
                    = carryForwardAccounts
                        .Where(t => t.Type == TransactionType.Expense.ToString() && t.SourceAccountId == selectedAccountId);

                carryForwardBalance += carryForwardAccounts
                    .Where(t => t.Type == TransactionType.Expense.ToString() && t.SourceAccountId == selectedAccountId)
                    .Sum(t => t.Amount);
            }

            // Step 2: Get balances for the current month
            var (currentCategories, currentAccounts) = c.FilterTransactions(accountDetails);
            decimal currentBalance = 0;

            if (currentCategories != null)
            {
                var t1 = currentCategories
                    .Where(t => t.Type == TransactionType.Income.ToString());

                currentBalance += currentCategories
                    .Where(t => t.Type == TransactionType.Income.ToString())
                    .Sum(t => t.Amount);
                var t2 = currentCategories
                    .Where(t => t.Type == TransactionType.Expense.ToString());
                currentBalance += currentCategories
                    .Where(t => t.Type == TransactionType.Expense.ToString())
                    .Sum(t => t.Amount);
            }

            if (currentAccounts != null)
            {
                var selectedAccountId = accountDetails.AccountId;

                var t1 = currentAccounts
                    .Where(t => t.Type == TransactionType.Income.ToString() && t.DestinationAccountId == selectedAccountId);
                currentBalance += currentAccounts
                    .Where(t => t.Type == TransactionType.Income.ToString() && t.DestinationAccountId == selectedAccountId)
                    .Sum(t => t.Amount);
                var t2 = currentAccounts
                    .Where(t => t.Type == TransactionType.Expense.ToString() && t.SourceAccountId == selectedAccountId);
                currentBalance += currentAccounts
                    .Where(t => t.Type == TransactionType.Expense.ToString() && t.SourceAccountId == selectedAccountId)
                    .Sum(t => t.Amount);
            }

            // Step 3: Final balance = carry forward + current month
            this.Balance = carryForwardBalance + currentBalance;
        }

        #endregion Private Methods

        #region Events
        private void OnBudgetChanged()
        {
            // For example: refresh data, re-calculate balance, etc.
        }

        [ObservableProperty]
        private string selectedBudget;
        private void LoadAvailableBudgets()
        {
            _isInitializing = true;

            var files = Directory.GetFiles(FileSystem.AppDataDirectory, "expenses_*.db");

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file).Replace("expenses_", "");
                AvailableBudgets.Add(name);
            }
            if (AvailableBudgets.Count == 0)
                AvailableBudgets.Add("initial");
        }


        [RelayCommand]
        private async Task CreateNewBudget()
        {
            var currentPage = Application.Current?.Windows[0]?.Page;
            if (currentPage != null)
            {
                var result = await currentPage.DisplayPromptAsync("New Budget", "Enter budget name:");
                if (string.IsNullOrWhiteSpace(result)) return;

                var dbPath = Path.Combine(FileSystem.AppDataDirectory, $"expenses_{result}.db");

                if (!File.Exists(dbPath))
                {
                    File.Create(dbPath).Close(); // Ensure file exists
                    AvailableBudgets.Add(result);
                    SelectedBudget = result;
                }
                else
                {
                    await currentPage.DisplayAlert("Exists", "Budget already exists.", "OK");
                }
                this.PublishFilterChanged(true);
            }
        }

        partial void OnSelectedBudgetChanged(string value)
        {
            // Skip logic when setting the value programmatically
            if (_isInitializing || value == null)
                return;
            if (!string.IsNullOrWhiteSpace(value))
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, $"expenses_{value}.db");
                _budgetContextService.SetBudget(dbPath);
                this.PublishFilterChanged(true);

            }
        }

        #endregion
    }
}
