using Core.Enum;
using Core.ViewModels;
using TrakiLibrary.Models;

namespace Core.Pages;

[QueryProperty(nameof(TypeString), "type")]
public partial class TransactionPage : ContentPage
{
    #region Private Variables
    private TransactionViewModel? _transactionViewModel;
    private Transaction? _transaction;
    #endregion Private Variables
    #region Constructor
    public TransactionPage(TransactionViewModel? transactionViewModel)
    {
		InitializeComponent();
        _transactionViewModel = transactionViewModel;
        BindingContext = transactionViewModel;
	}
    #endregion Constructor

    #region Property
    public Transaction? Transaction
    {
        get => _transaction;
        set => _transaction = value;
    }

    private string? _typeString;
    public string? TypeString
    {
        get => _typeString;
        set
        {
            _typeString = value;

            // Parse the string to enum
            if (System.Enum.TryParse<TransactionType>(value, out var result))
            {
                Type = result;
            }
        }
    }


    public TransactionType Type{ get; set; }
    //// Property to receive the 'type' parameter
    //public TransactionType Type
    //{
    //    get => _transactionViewModel.Type;
    //    set => _transactionViewModel.Type = value;
    //}
    #endregion Property

    #region Events
    // Handle receiving the transaction details for editing
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TransactionViewModel transactionViewModel)
        {
            if (Transaction != null)
            {
                // Load categories and set the selected category
                await transactionViewModel.LoadCategoriesAsync(Transaction.Category ?? new Category());

                // Load categories and set the selected category
                await transactionViewModel.LoadAccountsAsync(Transaction.Id);

                // Update other fields
                transactionViewModel.TransactionText = "Edit Transaction";
                transactionViewModel.Id = Transaction.Id;
                transactionViewModel.SelectedType = Type;
                transactionViewModel.Amount = Transaction.Amount;
                transactionViewModel.Reason = Transaction.Reason ?? string.Empty;   
                transactionViewModel.Date = Transaction.Date;
            }
            else
            {
                transactionViewModel.SelectedType = Type;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                await transactionViewModel.LoadCategoriesAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                await transactionViewModel.LoadAccountsAsync(0);
            }

        }
    }
    #endregion Events
}