using Core.Enum;
using Core.ViewModels;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Core.Pages;

[QueryProperty(nameof(TransactionByCategory), "Transaction")]
[QueryProperty(nameof(TypeString), "type")]
public partial class TransactionPage : ContentPage
{
    #region Private Variables
    private TransactionByCategory? _transactionByCategory;
    private ICategoryService? _categoryService;
    #endregion Private Variables

    #region Constructor
    public TransactionPage(TransactionViewModel? transactionViewModel, ICategoryService categoryService)
    {
		InitializeComponent();
        BindingContext = transactionViewModel;
        this._categoryService = categoryService;
	}
    #endregion Constructor

    #region Property
    public TransactionByCategory? TransactionByCategory
    {
        get => _transactionByCategory;
        set => _transactionByCategory = value;
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
            if (TransactionByCategory != null)
            {
                // Load categories and set the selected category
                await transactionViewModel.LoadCategoriesAsync(TransactionByCategory.Category ?? new Category());
                await transactionViewModel.LoadCategoriesAsync(TransactionByCategory.Category ?? new Category());

                if (_categoryService != null && this.TransactionByCategory.CategoryId != null)
                {
                    var category = await _categoryService.GetCategoryByIdAsync(this.TransactionByCategory.CategoryId!.Value);

                    if (category != null && transactionViewModel.ListOfCategories != null)
                    {
                        var matchedCategory = transactionViewModel.ListOfCategories
                            .FirstOrDefault(c => c.Id == category.Id);

                        // Use matchedCategory safely here
                        transactionViewModel.SelectedCategory = matchedCategory;
                    }
                }

                // Load categories and set the selected category
                await transactionViewModel.LoadAccountsAsync(TransactionByCategory.Id);

                // Update other fields
                transactionViewModel.TransactionText = "Edit Transaction";
                transactionViewModel.Id = TransactionByCategory.Id;
                transactionViewModel.SelectedType = Type;

                //if (_categoryService != null && this.Transaction.CategoryId != null)
                //{
                //    var category = await _categoryService.GetCategoryByIdAsync(this.Transaction.CategoryId.Value);
                //    transactionViewModel.SelectedCategory = category;
                //}
                transactionViewModel.Amount = TransactionByCategory.Amount;
                transactionViewModel.Reason = TransactionByCategory.Reason ?? string.Empty;   
                transactionViewModel.Date = TransactionByCategory.TransactionDate;
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