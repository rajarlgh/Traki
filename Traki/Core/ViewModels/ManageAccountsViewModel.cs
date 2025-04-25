using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Core.ViewModels
{
    public partial class ManageAccountsViewModel : ObservableObject
    {
        private readonly IAccountService _accountService;

#pragma warning disable
        [ObservableProperty]
        private ObservableCollection<Account> accounts = new();

        [ObservableProperty]
        private string newAccountName = string.Empty;

        [ObservableProperty]
        private int? accountId;
#pragma warning restore
        public ManageAccountsViewModel(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _ = LoadAccountsAsync(); // Run async method safely without warning
        }

        private static Page? GetCurrentPage() =>
            Application.Current?.Windows.FirstOrDefault()?.Page;

        private static async Task ShowAlertAsync(string title, string message, string cancel)
        {
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlert(title, message, cancel);
            }
        }

        [RelayCommand]
        public async Task LoadAccountsAsync()
        {
            try
            {
                var result = await _accountService.GetAccountsAsync();
                Accounts = new ObservableCollection<Account>(result ?? Enumerable.Empty<Account>());
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        public async Task AddOrUpdateAccountAsync()
        {
            if (string.IsNullOrWhiteSpace(NewAccountName))
            {
                await ShowAlertAsync("Error", "Account name cannot be empty.", "OK");
                return;
            }

            var account = new Account
            {
                Id = AccountId ?? 0,
                Name = NewAccountName
            };

            try
            {
                if (account.Id == 0)
                {
                    // Add new account
                    await _accountService.AddAccountAsync(account);

                    var savedAccounts = await _accountService.GetAccountsAsync();
                    var newAccount = savedAccounts?.LastOrDefault(a => a.Name == NewAccountName);
                    if (newAccount != null)
                    {
                        Accounts.Add(newAccount);
                    }
                }
                else
                {
                    // Update existing account
                    await _accountService.UpdateAccountAsync(account);

                    var existingAccount = Accounts.FirstOrDefault(a => a.Id == account.Id);
                    if (existingAccount != null)
                    {
                        existingAccount.Name = account.Name;
                        var index = Accounts.IndexOf(existingAccount);
                        Accounts[index] = existingAccount;
                    }
                }

                await ShowAlertAsync("Success", "Account saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message, "OK");
            }

            // Reset form
            NewAccountName = string.Empty;
            AccountId = null;
        }

        [RelayCommand]
        public Task EditAccountAsync(Account account)
        {
            if (account == null)
                return Task.CompletedTask;

            AccountId = account.Id;
            NewAccountName = account.Name ?? string.Empty;
            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task DeleteAccountAsync(Account? account)
        {
            if (account == null || !Accounts.Contains(account))
                return;

            var page = GetCurrentPage();
            if (page == null) return;

            var confirm = await page.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the account '{account.Name}'?",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    await _accountService.DeleteAccountAsync(account.Id);
                    Accounts.Remove(account);
                    await ShowAlertAsync("Success", "Account removed successfully.", "OK");
                }
                catch (Exception ex)
                {
                    await ShowAlertAsync("Error", ex.Message, "OK");
                }
            }
        }
    }
}
