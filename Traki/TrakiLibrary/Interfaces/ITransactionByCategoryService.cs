using TrakiLibrary.Models;

namespace TrakiLibrary.Interfaces
{
    public interface ITransactionByCategoryService
    {
        Task<List<TransactionByCategory>> GetTransactionsAsync();
        Task<List<TransactionByCategory>?> GetTransactionsByCategoryIdAsync(int id);
        Task<int> AddTransactionAsync(TransactionByCategory transaction);
        Task<int> DeleteTransactionAsync(int id);
        Task<int> UpdateTransactionAsync(TransactionByCategory transaction);
    }
}
