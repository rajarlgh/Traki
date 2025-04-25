using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Core.Shared;
using TrakiLibrary.Interfaces;

namespace Core.ViewModels
{
    public class IncomeViewModel : ObservableObject, IRecipient<FilterChangedMessage>
    {
        private readonly string _dbPath;
        private readonly IAccountService _accountService;

        public IncomeViewModel(string dbPath, IAccountService accountService)
        {
            _dbPath = dbPath;
            _accountService = accountService;
            WeakReferenceMessenger.Default.Register(this);
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
        }
    }

}
