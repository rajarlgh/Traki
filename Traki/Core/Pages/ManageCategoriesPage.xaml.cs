using Core.Enum;
using Core.ViewModels.PageViewModel;

namespace Core.Pages;

[QueryProperty(nameof(TransactionTypeString), "transactionType")]
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
            await ViewModel.InitializeAsync(this.TransactionType);
    }

    #region Property
    private string? _transactionTypeString;
    public string? TransactionTypeString
    {
        get => _transactionTypeString;
        set
        {
            _transactionTypeString = value;

            // Parse the string to enum
            if (System.Enum.TryParse<TransactionType>(value, out var result))
            {
                TransactionType = result;
            }
        }
    }

    public TransactionType TransactionType { get; set; }
    #endregion Property
}