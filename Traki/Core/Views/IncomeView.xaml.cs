using CommunityToolkit.Mvvm.Messaging;
using Core.Entity;
using Core.Enum;
using Core.Shared.Messages.Transactions;
using Core.ViewModels;

namespace Core.Views;

public partial class IncomeView : ContentView
{
    #region Private Variables
    private IncomeViewModel? _viewModel;
    #endregion Private Variables

    #region Constructor
    public IncomeView(IncomeViewModel viewModel)
	{
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    #endregion Constructor

    #region Events

    private void OnIncomeItemSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as ChartEntryWrapper;
        if (selectedItem != null)
        {
            var transactionMessage = new TransactionMessage() { CategoryId = selectedItem.CategoryId!.Value, TransactionType =  TransactionType.Income };

            // 1. Switch to Transactions Tab
            WeakReferenceMessenger.Default.Send(new SelectedTransactionMessage(transactionMessage));

            // 2. Clear selection
            collectionViewIncome.SelectedItem = null;
        }
        
        // Deselect the previously selected item in CollectionView2
        //var selectedWrapper = e.CurrentSelection.FirstOrDefault() as ChartEntryWrapper;
        //if (selectedWrapper != null)
        //{
        //    // Use the ID and Name from the wrapper
        //    this.SelectedCategory.Id = selectedWrapper.CategoryId;
        //    this.SelectedCategory.Name = selectedWrapper.Entry.Label;

        //    // Call the method in the ViewModel to fetch the breakdown

        //    this.CategoryType = "Income";
        //    var viewModel = BindingContext as TransactionHistoryViewModel;
        //    viewModel?.ShowBreakdownForCategory(this.SelectedCategory, this.CategoryType);
        //    collectionViewExpense.SelectedItems = null;
        //}
    }
    #endregion Events

}