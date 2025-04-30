using Core.ViewModels;
using TrakiLibrary.Models;

namespace Core.Pages;

public partial class TransactionPage : ContentPage
{
    #region Private Variables
    private TransactionViewModel _transactionViewModel;
    private Transaction _transaction;
    #endregion Private Variables
    #region Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TransactionPage(TransactionViewModel? transactionViewModel)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
		InitializeComponent();
	}
    #endregion Constructor

    #region Property
    public Transaction Transaction
    {
        get => _transaction;
        set => _transaction = value;
    }
    // Property to receive the 'type' parameter
    public string Type
    {
        get => _transactionViewModel.Type;
        set => _transactionViewModel.Type = value;
    }
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
                transactionViewModel.Type = Transaction.Type ?? string.Empty;
                transactionViewModel.Amount = Transaction.Amount;
                transactionViewModel.Reason = Transaction.Reason ?? string.Empty;   
                transactionViewModel.Date = Transaction.Date;
            }
            else
            {
                transactionViewModel.Type = Type;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                await transactionViewModel.LoadCategoriesAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                await transactionViewModel.LoadAccountsAsync(0);
            }

        }
    }
    #endregion Events
}