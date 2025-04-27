using Core.ViewModels;

namespace Core.Pages;

public partial class ExcelUploaderPage : ContentPage
{
    private ExcelUploaderViewModel? _viewModel;

    public ExcelUploaderPage(ExcelUploaderViewModel viewModel)
    {
        InitializeComponent();
        this._viewModel = viewModel;
        this.BindingContext = viewModel;
    }
}