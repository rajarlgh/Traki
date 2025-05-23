using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Core.Entity;
using Core.Enum;
using Core.Shared;
using System.Collections.ObjectModel;
using TrakiLibrary.Interfaces;

namespace Core.ViewModels
{
    public partial class ExpenseViewModel : TransactionBase, IRecipient<AccountChangedMessage>
    {
#pragma warning disable
        [ObservableProperty]
        private ObservableCollection<ChartEntryWrapper> expenseChartEntryWrappers;
#pragma warning restore

        #region Public Methods
        public void Receive(AccountChangedMessage accountChangedMessage)
        {
            var accountDetails = accountChangedMessage.Value;
            UpdateExpenseChart(accountDetails);
        }
        #endregion Public Methods

        #region Constructor
        public ExpenseViewModel(ICategoryService? _categoryService)
        {
            WeakReferenceMessenger.Default.Register <AccountChangedMessage>(this);
        }
        #endregion Constructor

        #region Private Methods
        private void UpdateExpenseChart(TransactionFilterRequest accountDetails)
        {
            var chart = new Chart();
            var data = chart.CreateChart(accountDetails, TransactionType.Expense);

            // Set collection of ChartEntryWrapper for CollectionView
            ExpenseChartEntryWrappers = new ObservableCollection<ChartEntryWrapper>(data);
            
        }
        #endregion Private Methods
    }
}
