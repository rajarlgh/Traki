using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enum;
using Core.Pages;
using System.Collections.ObjectModel;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Core.ViewModels
{
    public partial class DetailedTransactionsViewModel : ObservableObject
    {
        #region Private Variables
        private ICategoryService? _categoryService;
        private ITransactionByCategoryService? _transactionByCategoryService;
        #endregion Private Variables

        #region Property
#pragma warning disable
        [ObservableProperty]
        private ObservableCollection<TransactionByCategory>? selectedCategoryBreakdown;
#pragma warning restore

        #endregion Property

        #region Constructor
        public DetailedTransactionsViewModel(ICategoryService categoryService, ITransactionByCategoryService? transactionService)
        {
            _categoryService = categoryService;
            _transactionByCategoryService = transactionService;
        }
        #endregion Constructor

        #region Public Methods
        public async void ShowBreakdownForCategory(int? categoryId, TransactionType type)
        {
            if (_categoryService != null && categoryId != null && _transactionByCategoryService != null)
            {
                var category = await _categoryService.GetCategoryByIdAsync(categoryId.Value);
                var listOfTransactions =await _transactionByCategoryService.GetTransactionsByCategoryIdAsync(categoryId.Value);
                //var categoryId = category.Id;

                var breakdown = listOfTransactions?
                    .Where(t => t.Type == type.ToString() && t.CategoryId == categoryId)
                    .ToList();


                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (breakdown != null)
                        SelectedCategoryBreakdown = new ObservableCollection<TransactionByCategory>(breakdown);
                });

            }
        }
        #endregion Public Methods

        #region Commands
        [RelayCommand]
        public async Task EditTransactionDetailsAsync(TransactionByCategory transaction)
        {
            // Pass the TransactionViewModel to the transaction page
            await Shell.Current.GoToAsync($"{nameof(TransactionPage)}?type={transaction.Type}", true, new Dictionary<string, object>
            {
                { "Transaction", transaction }
            });
        }
        #endregion Commands
    }
}
