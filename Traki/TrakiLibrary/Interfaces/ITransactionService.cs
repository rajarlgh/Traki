using TrakiLibrary.Models;

namespace TrakiLibrary.Interfaces
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsAsync();
        Task<List<Transaction>?> GetTransactionsByCategoryIdAsync(int id);
        Task<int> AddTransactionAsync(Transaction transaction);
        Task<int> DeleteTransactionAsync(int id);
        Task<int> UpdateTransactionAsync(Transaction transaction);
    }
}
