using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Core.ViewModels
{
    public partial class TransactionBase : ObservableObject
    {
        #region Private Variables
#pragma warning disable
        [ObservableProperty]
        private bool isExpanded;
        [ObservableProperty]
        private bool showChartVisible;
#pragma warning restore
        #endregion Private Variables

        #region Commands
        [RelayCommand]
        private void ToggleFlags()
        {
            this.IsExpanded = !IsExpanded;
            this.ShowChartVisible = !this.IsExpanded;
        }
        #endregion Commands
    }
}
