using TrakiLibrary.Models;

namespace TrakiLibrary.Interfaces
{
    public interface ITransactionByAccountService
    {
        Task<List<TransactionByAccount>> GetTransactionsAsync();
        Task<List<TransactionByAccount>?> GetTransactionsByFromAccountIdAsync(int id);
        Task<int> AddTransactionAsync(TransactionByAccount transaction);
        Task<int> DeleteTransactionAsync(int id);
        Task<int> UpdateTransactionAsync(TransactionByAccount transaction);
    }
}
