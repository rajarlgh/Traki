using TrakiLibrary.Interfaces;

namespace Traki.Service
{
    // Services/BudgetContextService.cs
    // Traki.Service/BudgetContextService.cs
    public class BudgetContextService : IBudgetContextService
    {
        private string _currentDbPath = string.Empty;
        public string CurrentDbPath => _currentDbPath;
        public event Action? BudgetChanged;

        public void SetBudget(string dbPath)
        {
            if (_currentDbPath != dbPath)
            {
                _currentDbPath = dbPath;
                BudgetChanged?.Invoke();
            }
        }
    }


}
