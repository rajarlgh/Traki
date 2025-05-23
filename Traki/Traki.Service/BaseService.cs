using SQLite;

namespace Traki.Service
{
    public abstract class BaseService
    {
        protected bool _isInitialized = false;
        protected SQLiteAsyncConnection _database;
        protected string _currentDbPath;
        protected BudgetContextService _budgetContextService;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public BaseService(BudgetContextService budgetContextService)
        {
            _budgetContextService = budgetContextService;
            _budgetContextService.BudgetChanged += OnBudgetChanged;
            _currentDbPath = _budgetContextService.CurrentDbPath;
            InitializeDatabase(_currentDbPath);
        }

        protected void InitializeDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        protected virtual IEnumerable<Type> GetEntityTypes()
        {
            // Override in derived class to register tables
            return Enumerable.Empty<Type>();
        }

        protected virtual async Task EnsureInitializedAsync()
        {
            await _initLock.WaitAsync();
            try
            {
                var newDbPath = _budgetContextService.CurrentDbPath;

                if (_currentDbPath != newDbPath || !_isInitialized)
                {
                    InitializeDatabase(newDbPath);
                    _currentDbPath = newDbPath;

                    foreach (var type in GetEntityTypes())
                    {
                        await _database.CreateTableAsync(type);
                    }

                    _isInitialized = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }

        protected void OnBudgetChanged()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    _isInitialized = false;
                    await EnsureInitializedAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reinitializing DB: {ex.Message}");
                }
            });
        }
    }
}
