using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SQLite;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Transactions;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;


namespace Core.ViewModels
{
    public partial class SharedHeaderViewModel : ObservableObject
    {
        #region Filter 
#pragma warning disable 
        [ObservableProperty] private bool isIntervalFilterSelected;
        [ObservableProperty] private bool isDateFilterSelected;
        [ObservableProperty] private bool isMonthFilterSelected;
        [ObservableProperty] private bool isWeekFilterSelected;
        [ObservableProperty] private bool isYearFilterSelected;
        [ObservableProperty] private string? selectedFilterOption;
        [ObservableProperty]
        private bool isFilterExpanded = false;
        [ObservableProperty]
        private ObservableCollection<Account> listOfAccounts;
        [ObservableProperty]
        private Account selectedAccount;
        [ObservableProperty]
        private ObservableCollection<string> filterOptions = new()
        {
            "Day",
            "Week",
            "Month",
            "Year",
            "Interval",
            "Choose Date"
        };
        public ObservableCollection<string> Months { get; } = new(
       CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
.Where(m => !string.IsNullOrEmpty(m))
.ToList());

        public ObservableCollection<string> Weeks { get; } = new(
            Enumerable.Range(1, 52).Select(w => $"Week {w}").ToList());

        public ObservableCollection<int> Years { get; } = new(
            Enumerable.Range(DateTime.Now.Year - 5, 11).ToList()); // 5 years back & forward
        [ObservableProperty] private string selectedWeek;
        [ObservableProperty] private string selectedMonth;
        [ObservableProperty] private int selectedYear;

        [ObservableProperty]
        private DateTime fromDate = DateTime.Today;


        [ObservableProperty]
        private DateTime toDate = DateTime.Today;
        [ObservableProperty]
        private DateTime onDate;
#pragma warning restore 

        public bool IsAnyFilterVisible =>
            IsWeekFilterSelected || IsMonthFilterSelected || IsYearFilterSelected || IsIntervalFilterSelected || IsDateFilterSelected;

        partial void OnSelectedFilterOptionChanged(string? value)
        {
            IsDateFilterSelected = value == "Choose Date";
            IsIntervalFilterSelected = value == "Interval";
            IsMonthFilterSelected = value == "Month";
            IsWeekFilterSelected = value == "Week";
            IsYearFilterSelected = value == "Year";

            if (value == "Day")
            {
                var today = DateTime.Today;
                var endOfDay = today.AddDays(1).AddTicks(-1);
                // FilterTransactionsByRange(today, endOfDay);
            }
        }
        #endregion Filter

        #region DB
        private readonly SQLiteAsyncConnection? _database;
        private readonly IAccountService? _accountService;
        #endregion DB

        [RelayCommand]
        private void ToggleFilter()
        {
            IsFilterExpanded = !IsFilterExpanded;
        }


        //#region Collection
        //[ObservableProperty]
        //private ObservableCollection<Transaction> transactions;
        //private ObservableCollection<Transaction> allTransactions;

        //// Mark the property as observable
        //[ObservableProperty]
        //private ObservableCollection<Account> listOfAccounts;
        //// Mark the property as observable
        //[ObservableProperty]
        //private Account selectedAccount;
        //#endregion Collection
    }
}
