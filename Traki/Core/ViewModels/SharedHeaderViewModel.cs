using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.ObjectModel;
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
