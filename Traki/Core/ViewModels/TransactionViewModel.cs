using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enum;
using Core.Pages;
using System.Collections.ObjectModel;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Core.ViewModels
{
    public partial class TransactionViewModel : ObservableObject
    {
        #region Private Variables
        private readonly ICategoryService? _categoryService;
        private readonly IAccountService? _accountService;
        private readonly ITransactionByCategoryService? _transactionByCategoryService;
        #endregion Private Variables

        #region Constructors
        public TransactionViewModel(ICategoryService? categoryService, IAccountService? accountService, ITransactionByCategoryService? transactionByCategoryService )
        {
            this._categoryService = categoryService;
            this._accountService = accountService;
            this._transactionByCategoryService = transactionByCategoryService;
        }
        #endregion Constructors
#pragma warning disable

        public ObservableCollection<TransactionType> TransactionTypes { get; } =
              new ObservableCollection<TransactionType>(System.Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>());

        [ObservableProperty]
        private ObservableCollection<Category>? listOfCategories;
        [ObservableProperty]
        private Category? selectedCategory;

        [ObservableProperty]
        private ObservableCollection<Account>? listOfAccounts;
        [ObservableProperty]
        private Account? selectedAccount;
        [ObservableProperty]
        private string transactionText = "Transaction";
        [ObservableProperty]
        private int? id;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private string reason = string.Empty;

        [ObservableProperty]
        private TransactionType selectedType;
        [ObservableProperty]
        private DateTime date = DateTime.Now;


#pragma warning restore

        #region Property
        #endregion Property

        #region Public Methods
        public async Task LoadCategoriesAsync(Category? selectedCategory)
        {
            if (_categoryService != null)
            {
                var categories = await _categoryService.GetCategoriesAsync();

                // Filter categories by selectedType
                categories = categories.Where(c => c.Type == this.SelectedType.ToString()).ToList();

                // Create a new list with "Add New Category" inserted at the top
                var list = new List<Category>(categories);
                list.Insert(0, new Category { Id = -1, Name = "Add New Category" });

                ListOfCategories = new ObservableCollection<Category>(list);

                if (selectedCategory != null && selectedCategory.Id != 0 && ListOfCategories.Any())
                {
                    var matchedCategory = ListOfCategories.FirstOrDefault(c => c.Id == selectedCategory.Id);
                    if (matchedCategory != null)
                    {
                        SelectedCategory = matchedCategory;
                    }
                    else
                    {
                        SelectedCategory = null;
                    }
                }
                else
                {
                    SelectedCategory = null; // or keep default if needed
                }
            }
        }


        public async Task LoadAccountsAsync(int? accountId)
        {
            if (_accountService != null)
            {
                var accounts = await _accountService.GetAccountsAsync();

                var list = new List<Account>(accounts);
                list.Insert(0, new Account { Id = -1, Name = "Add New Account" });
                ListOfAccounts = new ObservableCollection<Account>(list);

                if (accountId != null && accountId != 0)
                {
                    var selectedAccount = ListOfAccounts.FirstOrDefault(a => a.Id == accountId);
                    if (selectedAccount != null)
                    {
                        SelectedAccount = selectedAccount;
                    }
                }
                else
                {
                    SelectedAccount = null; // or whatever default you want
                }
            }
        }


        #endregion Public Methods

        #region Events
        partial void OnSelectedCategoryChanged(Category? value)
        {
            if (value != null && value.Name == "Add New Category")
            {
                Shell.Current.GoToAsync($"{nameof(ManageCategoriesPage)}?transactionType={SelectedType}");
            }
        }
        partial void OnSelectedAccountChanged(Account? value)
        {
            if (value != null && value.Name == "Add New Account")
            {
                Shell.Current.GoToAsync(nameof(ManageAccountsPage));
            }
        }
        #endregion Events

        #region Commands
        [RelayCommand]
        public async Task AddTransactionAsync()
        {
            var transactionByCategory = new TransactionByCategory
            {
                Id = Id??0,
                Amount = Amount,
                Reason = Reason,
                Type = this.SelectedType.ToString(),
                Category = SelectedCategory,
                SourceAccountId = SelectedAccount?.Id ?? 0,
                TransactionDate = DateTime.Now
            };

            var currentPage = Application.Current?.Windows[0]?.Page;
            if (currentPage != null)
            {
                try
                {
                    if (_transactionByCategoryService != null)
                    {
                        if (transactionByCategory.Id == 0)
                            await _transactionByCategoryService.AddTransactionAsync(transactionByCategory);
                        else
                            await _transactionByCategoryService.UpdateTransactionAsync(transactionByCategory);

                        ResetTransactionForm();
                        await currentPage.DisplayAlert("Success", "Transaction saved successfully.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await currentPage.DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
        #endregion Commands

        #region Private Methods
        private void ResetTransactionForm()
        {
            Amount = 0;
            Reason = string.Empty;
            SelectedCategory = null;
            SelectedAccount = null;
        }
        #endregion Private Methods

    }
}
