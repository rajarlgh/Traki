using Core.ViewModels;

namespace Core.Views;

public partial class ExpenseView : ContentView
{
    #region Private Variables
    private ExpenseViewModel? _viewModel;
    #endregion Private Variables

    #region Constructor
    public ExpenseView(ExpenseViewModel viewModel)
	{
		InitializeComponent();
        this._viewModel = viewModel;
        BindingContext = _viewModel;
	}
    #endregion Constructor

    #region Events
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
    #endregion Events
}