using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Enum;
using Core.Shared;
using TrakiLibrary.Interfaces;

namespace Core.ViewModels
{
    public partial class IncomeViewModel : ObservableObject, IRecipient<FilterChangedMessage>
    {
        #region Private Variables
        private readonly string _dbPath;
        private readonly ITransactionService _transactionService;
        #endregion Private Variables

#pragma warning disable
        [ObservableProperty]
        private bool isIncomeExpanded = false;
#pragma warning restore

        #region Public Constructor
        public IncomeViewModel(string dbPath, ITransactionService transactionService)
        {
            _dbPath = dbPath;
            _transactionService = transactionService;
            //WeakReferenceMessenger.Default.Register<IncomeViewModel, FilterChangedMessage>(this, 
            //    (r, m) => r.Receive(m));

            //StrongReferenceMessenger.Default.Register<IncomeViewModel, FilterChangedMessage>(this, (r, m) => r.Receive(m));
            StrongReferenceMessenger.Default.Register(this);

        }
        #endregion Public Constructor

        #region Public Methods
        public void Receive(FilterChangedMessage message)
        {
            var filter = message.Value;
            // Use filter.SelectedFilterOption, etc. to update IncomeChartEntryWrappers
            UpdateIncomeChart(filter);
        }


        #endregion Public Methods

        #region Private Methods
        private void UpdateIncomeChart(FilterState filter)
        {
            // your logic to refresh IncomeChartEntryWrappers based on filter
            var transactions = _transactionService.GetTransactionsAsync().Result;
            var option = filter.SelectedFilterOption;
            var account = filter.SelectedAccount;

           
            if (option == FilterOption.All)
            {
                var year = string.Empty;

            }
            else if (option == FilterOption.Day)
            {
                var today = DateTime.Today; // midnight (00:00)
                var endOfDay = today.AddDays(1).AddTicks(-1); // 23:59:59.9999999
                FilterTransactionsByRange(today, endOfDay);
            }
            else if (option == FilterOption.Week)
            {
                var week = filter.SelectedWeek;
                var year = filter.SelectedYear;

            }
            else if (option == FilterOption.Month)
            {
                var month = filter.SelectedMonth;
                var year = filter.SelectedYear;
            }
            else if (option == FilterOption.Year)
            {
                var year = filter.SelectedYear;
            }
            else if (option == FilterOption.Interval)
            {
                var fromDate = filter.FromDate;
                var toDate = filter.ToDate;
            }
            else if (option == FilterOption.ChooseDate)
            {
                var fromDate = filter.FromDate;
                var toDate = filter.ToDate;
            }
            //var   filteredTransactions = transactions.Where(t => t.Date >= filter.Selec && t.Date <= endDate && t.AccountId == selectedAccount.Id);
            //else
            //    filteredTransactions = filteredTransactions.Where(t => t.Date >= startDate && t.Date <= endDate);
            //var data = filteredTransactions.ToListAsync().Result;
            //allTransactions = new ObservableCollection<Transaction>(data);

            //// Execute the query and update the Transactions list
            //Transactions = new ObservableCollection<Transaction>(data);

            //CalculateBalances();
            //OnPropertyChanged(nameof(Transactions));
            //this.LoadTransactionsAndSetGrid(Transactions);

        }

        private async void FilterTransactionsByRange(DateTime startDate, DateTime endDate)
        {
            //var filteredTransactions = _database.Table<Transaction>();
            //var t = filteredTransactions.ToListAsync().Result;
            //// Get record counts grouped by month from the database
            //// var dbMonthlyCounts = await GetRecordCountsByMonthFromDatabaseAsync();

            //if (selectedAccount != null && selectedAccount.Id > 0)
            //    filteredTransactions = filteredTransactions.Where(t => t.Date >= startDate && t.Date <= endDate && t.AccountId == selectedAccount.Id);
            //else
            //    filteredTransactions = filteredTransactions.Where(t => t.Date >= startDate && t.Date <= endDate);
            //var data = filteredTransactions.ToListAsync().Result;
            //allTransactions = new ObservableCollection<Transaction>(data);

            //// Execute the query and update the Transactions list
            //Transactions = new ObservableCollection<Transaction>(data);

            //CalculateBalances();
            //OnPropertyChanged(nameof(Transactions));
            //this.LoadTransactionsAndSetGrid(Transactions);

        }
        #endregion Private Methods

        #region Commands
        [RelayCommand]
        private void ToggleIncome()
        {
            IsIncomeExpanded = !IsIncomeExpanded;
        }
        #endregion 
    }

}
