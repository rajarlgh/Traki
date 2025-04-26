using Core.ViewModels;
using Core.Views;
using Traki.ViewModels;

namespace Traki.Pages;

public partial class DashboardPage : ContentPage
{
    private DashboardViewModel? _viewModel;

    public DashboardPage()
	{
		InitializeComponent();
	}

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (SharedHeaderControl != null)
            await SharedHeaderControl.ReloadAccountsAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (SharedHeaderControl.BindingContext is SharedHeaderViewModel vm)
        {
            vm.UnregisterMessenger();
        }
    }

}