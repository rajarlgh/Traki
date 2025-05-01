using SQLite;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Traki.Service
{
    public class CategoryService : ICategoryService
    {
        #region Private Variables
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initLock = new(1, 1); // Thread-safe initialization
        #endregion Private Variables

        #region public Methods
        public async Task InitializeAsync()
        {
            await _database.CreateTableAsync<Category>();
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            await EnsureInitializedAsync();
            return await _database.Table<Category>().ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Category>()
                                  .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Category> AddCategoryAsync(Category category)
        {
            await EnsureInitializedAsync(); // Critical to prevent hanging
            // Check if an account with the same name already exists
            var existingCategory = _database.Table<Category>()
                                                 .Where(a => a.Name == category.Name)
                                                 .FirstOrDefaultAsync().Result;
            if (existingCategory != null)
            {
                // Return the existing account with its ID
                return existingCategory;
            }

            // Insert the new account
            var t = _database.InsertAsync(category).Result;

            // Return the newly created account with its generated ID
            return category;
        }

        public async Task DeleteCategoryAsync(int? id)
        {
            await EnsureInitializedAsync(); // Critical to prevent hanging

            await _database.DeleteAsync<Category>(id);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await EnsureInitializedAsync(); // Critical to prevent hanging

            await _database.UpdateAsync(category);
        }

        #endregion public Methods

        #region Constructors
        public CategoryService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }
        #endregion Constructors



        #region Private Methods
        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized) return;

            await _initLock.WaitAsync();
            try
            {
                if (!_isInitialized)
                {
                    await _database.CreateTableAsync<Category>();
                    _isInitialized = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }
        #endregion Private Methods

        
    }
}
