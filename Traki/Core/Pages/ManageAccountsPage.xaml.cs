using Core.ViewModels;

namespace Core.Pages;

public partial class ManageAccountsPage : ContentPage
{
    private readonly ManageAccountsViewModel _viewModel;
    public ManageAccountsPage(ManageAccountsViewModel viewModel)
	{
		InitializeComponent();
        this._viewModel = viewModel;
        BindingContext = viewModel;
	}

}