using CommunityToolkit.Mvvm.ComponentModel;
using Core.Entity;
using Core.Shared;
using System.Collections.ObjectModel;

namespace Core.ViewModels
{
    public partial class ExpenseViewModel : ObservableObject
    {
#pragma warning disable
        [ObservableProperty]
        private ObservableCollection<ChartEntryWrapper> expenseChartEntryWrappers;
#pragma warning restore

        #region Public Methods
        public void Receive(FilterChangedMessage message)
        {
            var filter = message.Value;
            // Use filter.SelectedFilterOption, etc. to update IncomeChartEntryWrappers
            //UpdateIncomeChart(filter);
        }
        #endregion Public Methods
    }
}
