namespace Core.Views;

public partial class ExpenseView : ContentView
{
	public ExpenseView()
	{
		InitializeComponent();
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