using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Traki.Service
{
    public class TransactionByCategoryService : BaseService, ITransactionByCategoryService
    {
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public TransactionByCategoryService(BudgetContextService budgetContextService) : base(budgetContextService)
        {

        }

        protected override IEnumerable<Type> GetEntityTypes()
        {
            return new[] { typeof(TransactionByCategory) };
        }

        public async Task<List<TransactionByCategory>> GetTransactionsAsync()
        {
            await EnsureInitializedAsync();
                return await _database.Table<TransactionByCategory>().ToListAsync();
        }

        public async Task<List<TransactionByCategory>> GetTransactionsByCategoryIdAsync(int categoryId)
        {
            await EnsureInitializedAsync();
            return await _database.Table<TransactionByCategory>()
                                  .Where(t => t.CategoryId == categoryId)
                                  .ToListAsync();
        }

        public async Task<int> AddTransactionAsync(TransactionByCategory transaction)
        {
            await EnsureInitializedAsync();
            return await _database.InsertAsync(transaction);
        }

        public async Task<int> UpdateTransactionAsync(TransactionByCategory transaction)
        {
            await EnsureInitializedAsync();
            return await _database.UpdateAsync(transaction);
        }

        public async Task<int> DeleteTransactionAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.DeleteAsync<TransactionByCategory>(id);
        }

        //protected async override Task? EnsureInitializedAsync()
        //{
        //    if (_isInitialized) return;

        //    await _initLock.WaitAsync();
        //    try
        //    {
        //        var newDbPath = _budgetContextService.CurrentDbPath;

        //        if (_currentDbPath != newDbPath || !_isInitialized)
        //        {
        //            InitializeDatabase(newDbPath);
        //            _currentDbPath = newDbPath;

        //            await _database.CreateTableAsync<TransactionByCategory>();
        //            _isInitialized = true;
        //        }
        //    }
        //    finally
        //    {
        //        _initLock.Release();
        //    }
        //}
    }
}
