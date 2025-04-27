using Core.ViewModels;

namespace Core.Pages;

public partial class ExcelUploaderPage : ContentPage
{
    #region Private Variable
    private ExcelUploaderViewModel? _viewModel;
    #endregion Private Variable

    #region Constructor
    public ExcelUploaderPage(ExcelUploaderViewModel viewModel)
    {
        InitializeComponent();
        this._viewModel = viewModel;
        this.BindingContext = viewModel;
    }
    #endregion Constructor

    #region Event
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ExcelUploaderViewModel vm)
        {
            vm.Reset();
        }
    }
    #endregion Event

}