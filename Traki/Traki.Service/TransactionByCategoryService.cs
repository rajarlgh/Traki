using SQLite;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Traki.Service
{
    public class TransactionByCategoryService : ITransactionByCategoryService
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public TransactionByCategoryService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
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

        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized) return;

            await _initLock.WaitAsync();
            try
            {
                if (!_isInitialized)
                {
                    await _database.CreateTableAsync<TransactionByCategory>();
                    _isInitialized = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }
    }
}
