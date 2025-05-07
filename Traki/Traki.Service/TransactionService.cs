using SQLite;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Traki.Service
{
    public class TransactionService : ITransactionService
    {
        #region Private Variables
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initLock = new(1, 1); // Thread-safe initialization
        #endregion Private Variables

        #region Constructors
        public TransactionService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            //_database.CreateTableAsync<Transaction>().Wait();
        }
        #endregion Constructors

        #region Public Methods
        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            await EnsureInitializedAsync();
            return await _database.Table<Transaction>().ToListAsync();
        }

        public async Task<List<Transaction>?> GetTransactionsByCategoryIdAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Transaction>()
                                  .Where(t=> t.CategoryId == id).ToListAsync();
        }

        public async Task<int> AddTransactionAsync(Transaction transaction)
        {
            await EnsureInitializedAsync();

            return await _database.InsertAsync(transaction);
        }

        public async Task<int> DeleteTransactionAsync(int id)
        {
            await EnsureInitializedAsync();

            return await _database.DeleteAsync<Transaction>(id);
        }

        public async Task<int> UpdateTransactionAsync(Transaction transaction)
        {
            await EnsureInitializedAsync();

            return await _database.UpdateAsync(transaction);
        }
        #endregion Public Methods

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
