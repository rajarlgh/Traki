using TrakiLibrary.Models;

namespace TrakiLibrary.Interfaces
{
    public interface ICategoryService
    {
        Task InitializeAsync();

        Task<List<Category>> GetCategoriesAsync();
        Task<Category> AddCategoryAsync(Category category);

        Task DeleteCategoryAsync(int? id);
        Task UpdateCategoryAsync(Category category);
    }
}
