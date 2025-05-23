using Core.ViewModels.PageViewModel;

namespace Core.Pages;

public partial class ManageCategoriesPage : ContentPage
{
    private ManageCategoriesViewModel ViewModel => BindingContext as ManageCategoriesViewModel
    ?? throw new InvalidOperationException("BindingContext is not ManageCategoriesViewModel or is null.");

    public ManageCategoriesPage(ManageCategoriesViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
            await ViewModel.InitializeAsync();
    }
}