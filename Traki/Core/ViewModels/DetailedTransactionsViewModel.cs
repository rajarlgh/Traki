using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enum;
using System.Collections.ObjectModel;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Core.ViewModels
{
    public partial class DetailedTransactionsViewModel : ObservableObject
    {
        #region Private Variables
        private ICategoryService? _categoryService;
        private ITransactionService? _transactionService;
        #endregion Private Variables

        #region Property
#pragma warning disable
        [ObservableProperty]
        private ObservableCollection<Transaction>? selectedCategoryBreakdown;
#pragma warning restore

        #endregion Property

        #region Constructor
        public DetailedTransactionsViewModel(ICategoryService categoryService, ITransactionService? transactionService)
        {
            _categoryService = categoryService;
            _transactionService = transactionService;
        }
        #endregion Constructor

        #region Public Methods
        public async void ShowBreakdownForCategory(int? categoryId, TransactionType type)
        {
            if (_categoryService != null && categoryId != null && _transactionService != null)
            {
                var category = await _categoryService.GetCategoryByIdAsync(categoryId.Value);
                var listOfTransactions =await _transactionService.GetTransactionsByCategoryIdAsync(categoryId.Value);
                //var categoryId = category.Id;

                var breakdown = listOfTransactions?
                    .Where(t => t.Type == type.ToString() && t.CategoryId == categoryId)
                    .ToList();


                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (breakdown != null)
                        SelectedCategoryBreakdown = new ObservableCollection<Transaction>(breakdown);
                });

            }
        }
        #endregion Public Methods


    }
}
