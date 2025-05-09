using SQLite;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Traki.Service
{
    public class TransactionByAccountService : ITransactionByAccountService
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public TransactionByAccountService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        public async Task<List<TransactionByAccount>> GetTransactionsAsync()
        {
            await EnsureInitializedAsync();
            return await _database.Table<TransactionByAccount>().ToListAsync();
        }

        public async Task<List<TransactionByAccount>> GetTransactionsByFromAccountIdAsync(int fromAccountId)
        {
            await EnsureInitializedAsync();
            return await _database.Table<TransactionByAccount>()
                                  .Where(t => t.SourceAccountId == fromAccountId)
                                  .ToListAsync();
        }

        public async Task<List<TransactionByAccount>> GetTransactionsByToAccountIdAsync(int toAccountId)
        {
            await EnsureInitializedAsync();
            return await _database.Table<TransactionByAccount>()
                                  .Where(t => t.DestinationAccountId == toAccountId)
                                  .ToListAsync();
        }

        public async Task<int> AddTransactionAsync(TransactionByAccount transaction)
        {
            await EnsureInitializedAsync();
            return await _database.InsertAsync(transaction);
        }

        public async Task<int> UpdateTransactionAsync(TransactionByAccount transaction)
        {
            await EnsureInitializedAsync();
            return await _database.UpdateAsync(transaction);
        }

        public async Task<int> DeleteTransactionAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.DeleteAsync<TransactionByAccount>(id);
        }

        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized) return;

            await _initLock.WaitAsync();
            try
            {
                if (!_isInitialized)
                {
                    await _database.CreateTableAsync<TransactionByAccount>();
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
