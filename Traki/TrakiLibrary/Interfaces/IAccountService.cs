using TrakiLibrary.Models;

namespace TrakiLibrary.Interfaces
{
    public interface IAccountService
    {
        Task InitializeAsync();
        Task<List<Account>> GetAccountsAsync();
        Task<Account> AddAccountAsync(Account account);
        Task DeleteAccountAsync(int? id);
        Task UpdateAccountAsync(Account account);
    }
}
