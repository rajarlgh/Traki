using Core.ViewModels;

namespace Core.Views;

public partial class DetailedTransactionsView : ContentView
{
    #region Private Variable
    DetailedTransactionsViewModel? _viewModel;
    #endregion

    #region Constructor
    public DetailedTransactionsView(DetailedTransactionsViewModel? viewModel)
	{
		InitializeComponent();
        this._viewModel = viewModel;
        BindingContext = this._viewModel;
	}
    #endregion Constructor

}