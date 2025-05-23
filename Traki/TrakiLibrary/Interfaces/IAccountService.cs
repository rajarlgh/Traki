using TrakiLibrary.Models;

namespace TrakiLibrary.Interfaces
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccountsAsync();
        Task<List<Account>?> GetAccountByAccountIdAsync(int id);
        Task<Account?> GetAccountByAccountNameAsync(string accountName);
        Task<Account> AddAccountAsync(Account account);
        Task DeleteAccountAsync(int? id);
        Task UpdateAccountAsync(Account account);
    }
}
