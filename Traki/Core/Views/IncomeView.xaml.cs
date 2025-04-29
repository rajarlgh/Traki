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