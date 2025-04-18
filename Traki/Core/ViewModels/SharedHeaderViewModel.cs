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
        //#region Filter

        //[ObservableProperty] private bool isIntervalFilterSelected = false;
        //[ObservableProperty] private bool isDateFilterSelected = false;
        //[ObservableProperty] private bool isMonthFilterSelected;
        //[ObservableProperty] private bool isWeekFilterSelected;
        //[ObservableProperty] private bool isYearFilterSelected;
        //public bool IsAnyFilterVisible =>
        //            IsWeekFilterSelected || IsMonthFilterSelected || IsYearFilterSelected || IsIntervalFilterSelected || IsDateFilterSelected;

        //[ObservableProperty]
        //private string selectedFilterOption;// String.Empty;
        //partial void OnSelectedFilterOptionChanged(string value)
        //{

        //    IsDateFilterSelected = value == "Choose Date";
        //    IsIntervalFilterSelected = value == "Interval";
        //    IsMonthFilterSelected = value == "Month";
        //    IsWeekFilterSelected = value == "Week";
        //    IsYearFilterSelected = value == "Year";

        //    if (value == "Day")
        //    {
        //        var today = DateTime.Today; // midnight (00:00)
        //        var endOfDay = today.AddDays(1).AddTicks(-1); // 23:59:59.9999999
        //        //FilterTransactionsByRange(today, endOfDay);
        //    }


        //    //OnPropertyChanged(nameof(IsDateFilterSelected));
        //    //OnPropertyChanged(nameof(IsIntervalFilterSelected));
        //    //OnPropertyChanged(nameof(IsAnyFilterVisible));
        //}
        //#endregion Filter

        //#region DB
        //private readonly SQLiteAsyncConnection? _database;
        //private readonly IAccountService? _accountService;
        //#endregion DB

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
