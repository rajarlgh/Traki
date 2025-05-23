using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Traki.Service
{
    public class CategoryService : BaseService, ICategoryService
    {
        public CategoryService(BudgetContextService budgetContextService) : base(budgetContextService)
        {
 
        }

        protected override IEnumerable<Type> GetEntityTypes()
        {
            return new[] { typeof(Account) };
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            await EnsureInitializedAsync();
            return await _database.Table<Category>().ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Category>().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await EnsureInitializedAsync();
            var existingCategory = await _database.Table<Category>()
                                                  .Where(a => a.Name == category.Name)
                                                  .FirstOrDefaultAsync();

            if (existingCategory != null)
                return existingCategory;

            await _database.InsertAsync(category);
            return category;
        }

        public async Task DeleteCategoryAsync(int? id)
        {
            await EnsureInitializedAsync();
            await _database.DeleteAsync<Category>(id);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await EnsureInitializedAsync();
            await _database.UpdateAsync(category);
        }
    }
}
