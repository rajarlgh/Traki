using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Traki.Service
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(BudgetContextService budgetContextService) : base(budgetContextService)
        {
        }

        protected override IEnumerable<Type> GetEntityTypes()
        {
            return new[] { typeof(Account) };
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            await EnsureInitializedAsync();
            return await _database.Table<Account>().ToListAsync();
        }

        public async Task<List<Account>?> GetAccountByAccountIdAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Account>().Where(t => t.Id == id).ToListAsync();
        }

        public async Task<Account> GetAccountByAccountNameAsync(string accountName)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Account>()
                                  .Where(t => t.Name == accountName)
                                  .FirstOrDefaultAsync();
        }

        public async Task<Account> AddAccountAsync(Account account)
        {
            await EnsureInitializedAsync();

            var existingAccount = await _database.Table<Account>()
                                                 .Where(a => a.Name == account.Name)
                                                 .FirstOrDefaultAsync();

            if (existingAccount != null)
                return existingAccount;

            await _database.InsertAsync(account);
            return account;
        }

        public async Task DeleteAccountAsync(int? id)
        {
            await EnsureInitializedAsync();
            await _database.DeleteAsync<Account>(id);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            await EnsureInitializedAsync();
            await _database.UpdateAsync(account);
        }
    }
}
