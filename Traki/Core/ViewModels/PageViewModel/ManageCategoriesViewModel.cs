using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enum;
using System.Collections.ObjectModel;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Core.ViewModels.PageViewModel
{
    public partial class ManageCategoriesViewModel : ObservableObject
    {
        #region Private Variables
        private ICategoryService _categoryService;
        [ObservableProperty]
#pragma warning disable
        private ObservableCollection<Category> listOfCategories = new();
#pragma warning restore
        [ObservableProperty]
#pragma warning disable
        private int id;
#pragma warning restore
        [ObservableProperty]
#pragma warning disable
        private string newCategory = string.Empty;
#pragma warning restore
        #endregion Private Variables

        #region Constructor
        public ManageCategoriesViewModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;

        }
        #endregion Constructor

        #region Property
        public TransactionType TransactionType { get; set; }
        #endregion

        #region Public Methods
        public async Task InitializeAsync(TransactionType transactionType)
        {
            var categories = await _categoryService.GetCategoriesAsync();
            categories = categories.Where( r=> r.Type == transactionType.ToString()).ToList();
            this.ListOfCategories = new ObservableCollection<Category>();
            foreach (var category in categories)
            {
                this.ListOfCategories.Add(category);
            }
            this.TransactionType = transactionType;
        }
        #endregion

        #region Commands
        [RelayCommand]
        public Task EditCategoryAsync(Category category)
        {
            if (category == null)
                return Task.CompletedTask;

            Id = category.Id;
            this.NewCategory = category.Name ?? string.Empty;
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            if (!string.IsNullOrWhiteSpace(this.NewCategory))
            {
                var category = new Category
                {
                    Id = this.Id,
                    Name = this.NewCategory,
                    Type = this.TransactionType.ToString()
                };

                var currentPage = Application.Current?.Windows[0]?.Page;
                if (currentPage != null)
                {
                    try
                    {
                        if (category.Id == 0)
                        {
                            // Add new category
                            await _categoryService.AddCategoryAsync(category);

                            // Reload categories to get the latest ID
                            var savedCategories = await _categoryService.GetCategoriesAsync();
                            category = savedCategories.LastOrDefault(c => c.Name == NewCategory);
                            if (category != null)
                            {
                                // Add to observable collection
                                ListOfCategories.Add(category);
                            }
                        }
                        else
                        {
                            // Update existing category
                            await _categoryService.UpdateCategoryAsync(category);

                            // Find and update the category in the observable collection
                            var existingCategory = ListOfCategories.FirstOrDefault(c => c.Id == category.Id);
                            if (existingCategory != null)
                            {
                                existingCategory.Name = category.Name;

                                // Notify the UI of the change
                                var index = ListOfCategories.IndexOf(existingCategory);
                                ListOfCategories[index] = existingCategory;
                            }
                        }

                        // Show success message
                        await currentPage.DisplayAlert("Success", "Category saved successfully.", "OK");
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        await currentPage.DisplayAlert("Error", ex.Message, "OK");
                    }
                }
                // Reset form properties
                NewCategory = string.Empty;
                Id = 0;
            }
        }

        [RelayCommand]
        private async Task RemoveCategoryAsync(Category category)
        {
            if (category != null && ListOfCategories.Contains(category))
            {
                var currentPage = Application.Current?.Windows[0]?.Page;
                if (currentPage != null)
                {
                    try
                    {
                        await _categoryService.DeleteCategoryAsync(category.Id);
                        this.ListOfCategories.Remove(category);

                        await currentPage.DisplayAlert("Success", "Category removed successfully.", "OK");
                    }
                    catch (Exception ex)
                    {
                        await currentPage.DisplayAlert("Error", ex.Message, "OK");
                    }
                }
            }
        }
        #endregion Commands
    }
}
