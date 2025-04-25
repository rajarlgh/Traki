using Core.ViewModels;

namespace Core.Views;

public partial class IncomeView : ContentView
{
    private IncomeViewModel? _viewModel;
    public IncomeView(IncomeViewModel viewModel)
	{
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
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
    private void OnExpenseItemSelected(object sender, SelectionChangedEventArgs e)
    {
        //var selectedWrapper = e.CurrentSelection.FirstOrDefault() as ChartEntryWrapper;
        //if (selectedWrapper != null)
        //{
        //    // Use the ID and Name from the wrapper
        //    this.SelectedCategory.Id = selectedWrapper.CategoryId;
        //    this.SelectedCategory.Name = selectedWrapper.Entry.Label;

        //    this.CategoryType = "Expense";
        //    var viewModel = BindingContext as TransactionHistoryViewModel;
        //    viewModel?.ShowBreakdownForCategory(this.SelectedCategory, this.CategoryType);
        //    collectionViewIncome.SelectedItem = null;
        //}
    }
}