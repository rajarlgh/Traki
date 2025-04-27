using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Transactions;

namespace Core.ViewModels
{
    public partial class TransactionViewModel : ObservableObject
    {
#pragma warning disable 

        [ObservableProperty]
        private ObservableCollection<Transaction> selectedCategoryBreakdown;
#pragma warning restore 

    }
}
