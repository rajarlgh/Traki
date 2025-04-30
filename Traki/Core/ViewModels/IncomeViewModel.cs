using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Entity;
using Core.Enum;
using Core.Shared;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Core.ViewModels
{
    public partial class IncomeViewModel : TransactionBase, IRecipient<FilterChangedMessage>
    {
        #region Private Variables
        private readonly ITransactionService? _transactionService;
        private readonly ICategoryService? _categoryService;

        #endregion Private Variables

#pragma warning disable
        [ObservableProperty]
        private ObservableCollection<ChartEntryWrapper> incomeChartEntryWrappers;

        [ObservableProperty]
        private IncomeDisplayMode incomeDisplayMode = IncomeDisplayMode.Chart;
#pragma warning restore

        #region Public Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public IncomeViewModel(ITransactionService? transactionService, ICategoryService? categoryService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
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
        private void PublishAccountChanged(TransactionFilterRequest accountDetails)
        {
            StrongReferenceMessenger.Default.Send(new AccountChangedMessage((accountDetails)));
        }
        #endregion Public Methods

        #region Private Methods
        private void UpdateIncomeChart(FilterState filter)
        {
            // your logic to refresh IncomeChartEntryWrappers based on filter
            var option = filter.SelectedFilterOption;
            var account = filter.SelectedAccount;
            var year = filter.SelectedYear;
            var month = filter.SelectedMonth;
            var week = filter.SelectedWeek;

            var fromDate = filter.FromDate;
            var toDate = filter.ToDate;

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
                int y = 0;
                string w = string.Empty;
                if (filter != null)
                {
                    if (filter.SelectedYear != null)
                    {
                        y = filter.SelectedYear.Value;
                    }
                    if (filter.SelectedWeek != null)
                    {
                        w = filter.SelectedWeek;
                    }
                }
                var yr = y;

                var selectedWeek = w;

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
                if (filter != null)
                {
                    var mon = filter.SelectedMonth;
                    int yer = 0;
                    if (filter.SelectedYear != null)
                    {
                        yer = filter.SelectedYear.Value;
                    }
                    var yr = yer;

                    if (string.IsNullOrWhiteSpace(mon) || yr == 0)
                        return;

                    int m = DateTime.ParseExact(mon, "MMMM", CultureInfo.CurrentCulture).Month;
                    int y = yr;

                    fromDate = new DateTime(y, m, 1);
                    toDate = fromDate.AddMonths(1).AddDays(-1);
                }
            }
            else if (option == FilterOption.Year)
            {
                int y = 0;
                if (filter != null && filter.SelectedYear != null)
                {
                    y = filter.SelectedYear.Value;
                }
                var yr = y;
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

            int accountId = 0;
            if (filter != null && filter.SelectedAccount != null)
            {
                accountId = filter.SelectedAccount.Id;
            }

            FilterTransactionsByRange(fromDate, toDate, accountId, option);
        }

        private async void FilterTransactionsByRange(DateTime fromDate, DateTime toDate, int accountId, FilterOption? filterOption)
        {

            if (_transactionService == null)
                throw new InvalidOperationException("Transaction service is not initialized.");

            var transactions = await _transactionService.GetTransactionsAsync();

            if (transactions == null)
            {
                return;
            }
            if (_categoryService == null)
                throw new InvalidOperationException("Category service is not initialized.");

            var categories = await _categoryService.GetCategoriesAsync();
            var accountDetails = new TransactionFilterRequest() 
            { 
                Transactions = transactions, 
                FilterOption=filterOption,
                AccountId=accountId,
                FromDate=fromDate,
                ToDate=toDate,
                Categories = categories
            };
            this.PublishAccountChanged(accountDetails);
            var chart = new Chart();
            var data = chart.CreateChart(accountDetails, TransactionType.Income);
            IncomeChartEntryWrappers = new ObservableCollection<ChartEntryWrapper>(data);

            //var filteredTransactions = new List<Transaction>();

            //if (filterOption != FilterOption.All)
            //{
            //    if (accountId > 0)
            //    {
            //        filteredTransactions = transactions
            //            .Where(t => t.Date >= fromDate && t.Date <= toDate && t.AccountId == accountId)
            //            .ToList();
            //    }
            //    else
            //    {
            //        filteredTransactions = transactions
            //            .Where(t => t.Date >= fromDate && t.Date <= toDate)
            //            .ToList();
            //    }
            //}
            //else
            //{ filteredTransactions = transactions; }


            //var incomeGroupedData = filteredTransactions
            //    .Where( t => t.Type == "Income" && t.CategoryId != null)
            //    .GroupBy(t => t.CategoryId)
            //    .Select(g =>
            //    {
            //        var categoryId = g.Key;
            //        var categoryName = categories.FirstOrDefault(c => c.Id == categoryId)?.Name;

            //        return new
            //        {
            //            CategoryId = categoryId,
            //            CategoryName = categoryName,
            //            TotalAmount = g.Sum(t => t.Amount)
            //        };
            //    })
            //    .ToList();

            //var t = incomeGroupedData.Count();
            //var chartColor = new Chart();
            //var incomeData = incomeGroupedData.Select(data => new ChartEntryWrapper
            //{

            //    Label = data.CategoryName,
            //    ValueLabel = data.TotalAmount.ToString("F0"),
            //    Value = (float)data.TotalAmount, // 👈 SET THIS
            //    Color = chartColor.GetCategoryColor(data.CategoryName ?? string.Empty),
            //    CategoryId = data.CategoryId,
            //}).ToList();

            //// Set collection of ChartEntryWrapper for CollectionView
            //IncomeChartEntryWrappers = new ObservableCollection<ChartEntryWrapper>(incomeData);

        }


        #endregion Private Methods

 
    }
}
