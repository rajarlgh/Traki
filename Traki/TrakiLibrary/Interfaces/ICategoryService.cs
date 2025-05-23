using TrakiLibrary.Models;

namespace TrakiLibrary.Interfaces
{
    public interface ICategoryService
    {

        Task<List<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> AddCategoryAsync(Category category);

        Task DeleteCategoryAsync(int? id);
        Task UpdateCategoryAsync(Category category);
    }
}
