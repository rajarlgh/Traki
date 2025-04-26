using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Core.Shared;
using TrakiLibrary.Interfaces;

namespace Core.ViewModels
{
    public class IncomeViewModel : ObservableObject, IRecipient<FilterChangedMessage>
    {
        private readonly string _dbPath;
        private readonly ITransactionService _transactionService;

        public IncomeViewModel(string dbPath, ITransactionService transactionService)
        {
            _dbPath = dbPath;
            _transactionService = transactionService;
            //WeakReferenceMessenger.Default.Register<IncomeViewModel, FilterChangedMessage>(this, 
            //    (r, m) => r.Receive(m));

            //StrongReferenceMessenger.Default.Register<IncomeViewModel, FilterChangedMessage>(this, (r, m) => r.Receive(m));
            StrongReferenceMessenger.Default.Register(this);

        }

        public void Receive(FilterChangedMessage message)
        {
            var filter = message.Value;
            // Use filter.SelectedFilterOption, etc. to update IncomeChartEntryWrappers
            UpdateIncomeChart(filter);
        }

        private void UpdateIncomeChart(FilterState filter)
        {
            // your logic to refresh IncomeChartEntryWrappers based on filter
            var transactions = _transactionService.GetTransactionsAsync().Result;

        }

        public async void FilterTransactionsByRange(DateTime startDate, DateTime endDate)
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
    }

}
