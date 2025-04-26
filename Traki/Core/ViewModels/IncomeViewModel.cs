using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Enum;
using Core.Shared;
using System.Globalization;
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
            var year = filter.SelectedYear;
            var month = filter.SelectedMonth;
            var week = filter.SelectedWeek;

            var fromDate = filter.FromDate;
            var toDate = filter.ToDate;

            var filteredTransactions = transactions.Where(t => t.Date >= fromDate && t.Date <= toDate && t.AccountId == filter.SelectedAccount.Id);

            

            if (option == FilterOption.All)
            {
                year = 0;
            }
            else if (option == FilterOption.Day)
            {
                var today = DateTime.Today; // midnight (00:00)
                var endOfDay = today.AddDays(1).AddTicks(-1); // 23:59:59.9999999
            }
            else if (option == FilterOption.Week)
            {
                var yr = filter.SelectedYear.Value;

                var selectedWeek = filter.SelectedWeek;

                if (string.IsNullOrWhiteSpace(selectedWeek) || yr == 0)
                    return;

                if (!int.TryParse(selectedWeek.Replace("Week ", ""), out int parsedWeek) || parsedWeek <= 0)
                    return;

                // Get the first Monday of the year
                DateTime jan1 = new DateTime(yr, 1, 1);   
                int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
                DateTime firstMonday = jan1.AddDays(daysOffset >= 0 ? daysOffset : daysOffset + 7);

                DateTime startOfWeek = firstMonday.AddDays(((int.Parse(selectedWeek))- 1) * 7);
                DateTime endOfWeek = startOfWeek.AddDays(6);

                // Clamp to correct year boundaries
                if (startOfWeek.Year < yr) startOfWeek = new DateTime(yr, 1, 1);
                if (endOfWeek.Year > yr) endOfWeek = new DateTime(yr, 12, 31);

            }
            else if (option == FilterOption.Month)
            {
                var mon = filter.SelectedMonth;
                var yr = filter.SelectedYear;

                if (string.IsNullOrWhiteSpace(mon) || yr == 0)
                    return;

                int m = DateTime.ParseExact(mon, "MMMM", CultureInfo.CurrentCulture).Month;
                int y = yr.Value;

                fromDate = new DateTime(y, m, 1);
                toDate = fromDate.AddMonths(1).AddDays(-1);
            }
            else if (option == FilterOption.Year)
            {
                var yr = filter.SelectedYear.Value;
                fromDate = new DateTime(yr, 1, 1);
                toDate = new DateTime(yr, 12, 31);
            }
            else if (option == FilterOption.Interval)
            {
                fromDate = filter.FromDate;
                toDate = filter.ToDate;
            }
            else if (option == FilterOption.ChooseDate)
            {
                fromDate = filter.FromDate;
                toDate = filter.ToDate;
            }

            FilterTransactionsByRange(fromDate, toDate);


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
