using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Core.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        //[ObservableProperty]
        //private View selectedTabView;

        [RelayCommand]
        private void ShowIncome()
        {
            //SelectedTabView = new IncomeView();
        }
    }
}
