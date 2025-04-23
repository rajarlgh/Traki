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
}