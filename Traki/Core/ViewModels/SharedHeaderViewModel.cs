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
        #region Private Variable
        private bool _isInitializing;
        #endregion Private Variable

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
            if (this._isInitializing)
                return;
            _ = HandleSelectedFilterOptionChangedAsync(value);

        }
        private async Task HandleSelectedFilterOptionChangedAsync(FilterOption? value)
        {
            if (value != null && value == FilterOption.All)
            {
                if (SelectedAccount != null && SelectedAccount.Id == 0)
                   await this.LoadAccountsAsync(-1);
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
            _isInitializing = true;
            if (_accountService != null)
            {
                var accounts = await _accountService.GetAccountsAsync();
                var lstAccount = new ObservableCollection<Account>(accounts);
                lstAccount.Add(new Account { Id = -1, Name = "All" });
                lstAccount.Add(new Account { Id = -2, Name = "Add New Account" });

                this.ListOfAccounts = lstAccount;

                var selectedAccount = accounts.FirstOrDefault(r => r.Id == accountId);
                if (selectedAccount != null)
                {
                    SelectedAccount = lstAccount.FirstOrDefault(a => a.Id == selectedAccount.Id);
                }
                else
                {
                    SelectedAccount = lstAccount.FirstOrDefault(a => a.Id == -1);
                }

                _isInitializing = false;
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
            // Skip logic when setting the value programmatically
            if (_isInitializing || value == null)
                return;

            if (value != null)
            {
                if (value.Name == "Add New Account")
                {
                    Shell.Current.GoToAsync(nameof(ManageAccountsPage));
                    return; // Early return to avoid publishing filter unnecessarily
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
