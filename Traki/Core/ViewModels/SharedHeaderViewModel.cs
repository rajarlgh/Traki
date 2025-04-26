using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
        [ObservableProperty] private string? selectedFilterOption;
        [ObservableProperty]
        private bool isFilterExpanded = false;
        [ObservableProperty]
        private ObservableCollection<Account>? listOfAccounts = new();
        [ObservableProperty]
        private Account? selectedAccount = new();
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
        [ObservableProperty] private string selectedWeek = string.Empty;
        [ObservableProperty] private string selectedMonth = string.Empty;
        [ObservableProperty] private int selectedYear = 0;

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
            // your logic...
            PublishFilterChanged();
        }

        #endregion Filter

        #region DB
        private readonly IAccountService? _accountService;
        #endregion DB

        #region Constructor
        public SharedHeaderViewModel(string dbPath, IAccountService accountService)
        {
            this._accountService = accountService;
        }
        #endregion Constructor

        [RelayCommand]
        private async Task ToggleFilter()
        {
            IsFilterExpanded = !IsFilterExpanded;
            await this.LoadAccountsAsync(0);
        }

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

        private void PublishFilterChanged()
        {
            var filterState = new FilterState
            {
                SelectedFilterOption = SelectedFilterOption,
                SelectedMonth = SelectedMonth,
                SelectedWeek = SelectedWeek,
                SelectedYear = SelectedYear,
                FromDate = FromDate,
                ToDate = ToDate,
                OnDate = OnDate,
                SelectedAccount = SelectedAccount
            };

            //WeakReferenceMessenger.Default.Send(new FilterChangedMessage(filterState));
            StrongReferenceMessenger.Default.Send(new FilterChangedMessage((filterState)));

        }

        public void UnregisterMessenger()
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }

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

                PublishFilterChanged();
            }
            //SelectedCategoryBreakdown = null;
            //this.RefreshDataAsync();
        }

    }
}
