namespace TrakiLibrary.Interfaces
{
    // TrakiLibrary/Interfaces/IBudgetContextService.cs
    public interface IBudgetContextService
    {
        string CurrentDbPath { get; }
        event Action? BudgetChanged;
        void SetBudget(string dbPath);
    }
}
