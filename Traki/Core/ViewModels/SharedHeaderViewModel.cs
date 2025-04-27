using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Enum;
using Core.Pages;
using Core.Shared;
using Core.Views;
using Microsoft.Maui.ApplicationModel.Communication;
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
        [ObservableProperty] private FilterOption? selectedFilterOption;
        [ObservableProperty]
        private bool isFilterExpanded = false;
        [ObservableProperty]
        private ObservableCollection<Account>? listOfAccounts = new();
        [ObservableProperty]
        private Account? selectedAccount = new();
        //[ObservableProperty]
        //private ObservableCollection<string> filterOptions = new()
        //{
        //    "All",
        //    "Day",
        //    "Week",
        //    "Month",
        //    "Year",
        //    "Interval",
        //    "Choose Date"
        //};
        public ObservableCollection<string> Months { get; } = new(
       CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
.Where(m => !string.IsNullOrEmpty(m))
.ToList());

        public ObservableCollection<string> Weeks { get; } = new(
            Enumerable.Range(1, 52).Select(w => $"Week {w}").ToList());

        public ObservableCollection<int> Years { get; } = new(
            Enumerable.Range(DateTime.Now.Year - 5, 11).ToList()); // 5 years back & forward
        [ObservableProperty] private string selectedWeek = string.Empty;
        [ObservableProperty] private string selectedMonth = string.Empty;
        [ObservableProperty] private int selectedYear = 0;

        [ObservableProperty]
        private DateTime fromDate = DateTime.Today;


        [ObservableProperty]
        private DateTime toDate = DateTime.Today;
        [ObservableProperty]
        private DateTime onDate;

        [ObservableProperty]
        private bool isAnyFilterVisible =>
            IsWeekFilterSelected || IsMonthFilterSelected || IsYearFilterSelected || IsIntervalFilterSelected || IsDateFilterSelected;
#pragma warning restore


        public ObservableCollection<FilterOption> FilterOptions { get; } = new(
   System.Enum.GetValues(typeof(FilterOption)).Cast<FilterOption>());

        partial void OnSelectedFilterOptionChanged(FilterOption? value)
        {
            if (value != null && value == FilterOption.All)
            {
                var filterState = new FilterState
                {
                    SelectedFilterOption = selectedFilterOption,
                    SelectedAccount = SelectedAccount,

                    SelectedMonth = SelectedMonth,
                    SelectedWeek = SelectedWeek,
                    SelectedYear = SelectedYear,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    OnDate = OnDate,
                };
                // Notify the client
                PublishFilterChanged(filterState);
            }

            IsDateFilterSelected = value == FilterOption.ChooseDate;
            IsIntervalFilterSelected = value == FilterOption.Interval;
            IsMonthFilterSelected = value == FilterOption.Month;
            IsWeekFilterSelected = value == FilterOption.Week;
            IsYearFilterSelected = value == FilterOption.Year;
        }

        #endregion Filter

        #region DB
        private readonly IAccountService? _accountService;
        #endregion DB

        #region Constructor
        public SharedHeaderViewModel(IAccountService accountService)
        {
            this._accountService = accountService;
            // Select the first value by default
            SelectedFilterOption = FilterOptions.First();
        }
        #endregion Constructor

        #region Private Methods
        [RelayCommand]
        private async Task ToggleFilter()
        {
            IsFilterExpanded = !IsFilterExpanded;
            await this.LoadAccountsAsync(0);
        }

        private void PublishFilterChanged(FilterState filterState)
        {
            //WeakReferenceMessenger.Default.Send(new FilterChangedMessage(filterState));
            StrongReferenceMessenger.Default.Send(new FilterChangedMessage((filterState)));
        }

        #endregion Private Methods

        #region Public Methods
        public async Task LoadAccountsAsync(int accountId)
        {
            if (_accountService != null)
            {
                var accounts = await _accountService.GetAccountsAsync();
                this.ListOfAccounts = new ObservableCollection<Account>(accounts);
                this.ListOfAccounts.Add(new Account { Id = -1, Name = "All" });
                this.ListOfAccounts.Add(new Account { Id = -2, Name = "Add New Account" });
                OnPropertyChanged(nameof(ListOfAccounts));
                var selectedAccount = accounts.FirstOrDefault(r => r.Id == accountId);
                if (selectedAccount != null)
                {
                    SelectedAccount = ListOfAccounts.FirstOrDefault(a => a.Id == selectedAccount.Id);
                }

                else
                    SelectedAccount = this.ListOfAccounts.FirstOrDefault(a => a.Id == -1);
                OnPropertyChanged(nameof(SelectedAccount));
            }
        }

        public void UnregisterMessenger()
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }

        #endregion Public Methods

        #region Events

        partial void OnSelectedAccountChanged(Account? value)
        {
            if (value != null && value.Id != -1) /*All - Selected in Account dropdown.*/
            {
                //    var account = this._accountService??.GetAccountsAsync();

                //    var trans = allTransactions.Where(r => r.AccountId == value.Id).ToList();
                //    Transactions = new ObservableCollection<Transaction>(trans);
                //}
                //else
                //{
                //    Transactions = new ObservableCollection<Transaction>(allTransactions);
                //}
                //LoadTransactionsAndSetGrid(Transactions);
                //CalculateBalances();
                if (value != null && value.Name == "Add New Account")
                {
                    Shell.Current.GoToAsync(nameof(ManageAccountsPage));
                }


                var filterState = new FilterState
                {
                    SelectedFilterOption = selectedFilterOption,
                    SelectedAccount = SelectedAccount,

                    SelectedMonth = SelectedMonth,
                    SelectedWeek = SelectedWeek,
                    SelectedYear = SelectedYear,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    OnDate = OnDate,
                };
                PublishFilterChanged(filterState);
            }
            //SelectedCategoryBreakdown = null;
            //this.RefreshDataAsync();
        }

        #endregion Events

        #region Commands
        [RelayCommand]
        public void FilterTransactions()
        {
                var filterState = new FilterState
                {
                    SelectedFilterOption = this.SelectedFilterOption,
                    SelectedAccount = SelectedAccount,
                    SelectedWeek = SelectedWeek,
                    SelectedYear = SelectedYear,

                    SelectedMonth = SelectedMonth,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    OnDate = OnDate,
                };
                PublishFilterChanged(filterState);
            //FilterTransactionsByRange(startOfWeek, endOfWeek);
        }
        #endregion

    }
}
