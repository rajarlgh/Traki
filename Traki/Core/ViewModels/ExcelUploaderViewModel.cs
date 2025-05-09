using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enum;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using TrakiLibrary.Interfaces;
using TrakiLibrary.Models;

namespace Core.ViewModels
{
    public partial class ExcelUploaderViewModel : ObservableObject
    {

        #region Private Variables
        private readonly ITransactionService? _transactionService;
        private readonly IAccountService? _accountService;
        private readonly ICategoryService? _categoryService;
        private readonly ITransactionByCategoryService _trakiTransactionByCategoryService;
        private readonly ITransactionByAccountService _trakiTransactionByAccountService;
        #endregion Private Variables

        #region Observable Property
#pragma warning disable
        [ObservableProperty]
        private string? selectedFilePath; //SelectedFilePath

        [ObservableProperty]
        private bool canUpload; //CanUpload

        [ObservableProperty]
        private bool isUploading; //IsUploading

        [ObservableProperty]
        private double uploadProgress; //UploadProgress

        [ObservableProperty]
        private string? uploadStatus; //UploadProgress
#pragma warning restore

        #endregion Observable Property

        #region Constructor
        public ExcelUploaderViewModel(ITransactionService transactionService, IAccountService accountService, ICategoryService categoryService, ITransactionByCategoryService trakiTransactionByCategoryService, ITransactionByAccountService transactionByAccountService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
            _categoryService = categoryService;
            CanUpload = false; // Upload button is disabled initially
            _categoryService = categoryService;
            _trakiTransactionByCategoryService = trakiTransactionByCategoryService;
            _trakiTransactionByAccountService = transactionByAccountService;
        }
        #endregion Constructor

        #region Command

        [RelayCommand]
        private async Task BrowseAsync() //BrowseCommand
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                         { DevicePlatform.Android, new[] { "*/*" } } // ✅ Allows all file types
                    }),
                PickerTitle = "Select an Excel or CSV File"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                SelectedFilePath = result.FullPath;
                CanUpload = true;
            }
        }

        [RelayCommand]
        private async Task UploadAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedFilePath))
                {
                    await ShowErrorAsync("No file selected!");
                    return;
                }

                IsUploading = true;
                UploadStatus = "Reading file...";
                UploadProgress = 0;

                var transactions = await Task.Run(() => ReadCsv(SelectedFilePath));

                int total = transactions.Count;
                int batchSize = 100; // Set batch size to process records in smaller groups
                int batchCount = (total + batchSize - 1) / batchSize; // Calculate the number of batches
                int index = 0;

                foreach (var transaction in transactionByCategorys)
                {
                    _trakiTransactionByCategoryService.AddTransactionAsync(transaction);
                }

                foreach (var transaction in transactionByAccounts)
                {
                    _trakiTransactionByAccountService.AddTransactionAsync(transaction);
                }

                // Process transactions in batches
                for (int i = 0; i < batchCount; i++)
                {
                    var batch = transactions.Skip(i * batchSize).Take(batchSize).ToList();
                    var batchTasks = new List<Task>();

                    foreach (var transaction in batch)
                    {
                        if (_transactionService != null)
                            batchTasks.Add(_transactionService.AddTransactionAsync(transaction));
                    }

                    // Await all tasks in the current batch concurrently
                    await Task.WhenAll(batchTasks);

                    // Update progress after each batch
                    index += batch.Count;
                    UploadProgress = (double)index / total;
                    UploadStatus = $"Uploading... {index} of {total}";
                }

                UploadStatus = "Upload completed!";
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to upload file: {ex.Message}");
            }
            finally
            {
                await Task.Delay(1000);
                IsUploading = false;
            }
        }
        #endregion Command

        #region Private Methods
        private async Task ShowErrorAsync(string message)
        {
            var page = Application.Current?.Windows[0].Page;
            if (page is not null)
            {
                await page.DisplayAlert("Error", message, "OK");
            }
        }


        List<TransactionByCategory> transactionByCategorys = new List<TransactionByCategory>();
        List<TransactionByAccount> transactionByAccounts = new List<TransactionByAccount>();

        private async Task<List<Transaction>> ReadCsv(string filePath)
        {
            var transactions = new List<Transaction>();
            var rejectedTransactions = new List<Transaction>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            if (_accountService == null)
                return transactions;

            var existingAccounts = (await _accountService.GetAccountsAsync())
                .Where(a => !string.IsNullOrEmpty(a.Name)) // filter out invalid accounts
                .ToDictionary(a => a.Name!, a => a);        // '!' because after filter it's safe

            if (_categoryService == null)
                return transactions;

            var existingCategories = (await _categoryService.GetCategoriesAsync())
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .ToDictionary(c => c.Name!, c => c);

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            while (csv.Read())
            {
                // Skip the header
                if (csv.GetField((int)FileColumns.Date)?.ToLower() == "date")
                    continue;

                var transactionDate = DateTime.ParseExact(
                            csv.GetField((int)FileColumns.Date) ?? throw new InvalidOperationException("Date field is missing."),
                            "d/M/yyyy",
                            CultureInfo.InvariantCulture);

                var accountName = csv.GetField((int)FileColumns.AccountName);
                var categoryOrTransferInfo = csv.GetField((int)FileColumns.CategoryName);
                var amountValue = decimal.Parse(csv.GetField((int)FileColumns.Amount) ?? "0");
                var currency = csv.GetField((int)FileColumns.Currency);

                // Convert the Amount to positive value if it’s a transfer
                //if (categoryOrTransferInfo != null && (categoryOrTransferInfo.Contains("From '") || categoryOrTransferInfo.Contains("To '")))
                //{
                //    amountValue = (amountValue < 0) ? amountValue * -1 : amountValue;
                //}

                var description = csv.GetField((int)FileColumns.Description);

                // Get or create an account
                if (!existingAccounts.TryGetValue(accountName ?? string.Empty, out var account) && accountName != null && currency != null)
                {
                    decimal initialAccBalance = 0;
                    // Update account balance if needed
                    if (categoryOrTransferInfo != null && categoryOrTransferInfo.Contains("Initial balance '") && account != null)
                    {
                        initialAccBalance = amountValue;
                    }
                    account = await _accountService.AddAccountAsync(new Account { Name = accountName, Currency = currency, InititalAccBalance = initialAccBalance });
                    existingAccounts[accountName ?? string.Empty] = account;
                }

                // Get or derive transferred account (ToAccount)
                var transferredAccount = await DeriveAccountAsync(categoryOrTransferInfo ?? string.Empty, currency ?? string.Empty, existingAccounts);


                // Create the Category if not exists.
                Category? category = null;
                if (!string.IsNullOrWhiteSpace(categoryOrTransferInfo) &&
                    !categoryOrTransferInfo.Contains("From '") &&
                    !categoryOrTransferInfo.Contains("To '") &&
                    !categoryOrTransferInfo.Contains("Initial balance '"))
                {
                    if (!existingCategories.TryGetValue(categoryOrTransferInfo, out category))
                    {
                        var categoryType = amountValue > 0 ? "Income" : "Expense";

                        category = await _categoryService.AddCategoryAsync(new Category
                        {
                            Name = categoryOrTransferInfo,
                            Type = categoryType
                        });

                        existingCategories[categoryOrTransferInfo] = category;
                    }
                }

                bool isRecordAlreadyExists = false;
                if (categoryOrTransferInfo != null && categoryOrTransferInfo.Contains("From '") && accountName != null)
                {
                    string transferAccountNamePart = categoryOrTransferInfo.Replace("From '", "");
                    var transferAccountName = transferAccountNamePart.Substring(0, transferAccountNamePart.Length - 1);
                    var sourceAccount = await _accountService.GetAccountByAccountNameAsync(transferAccountName);

                    var destinationAccount = await _accountService.GetAccountByAccountNameAsync(accountName);

                    if (_transactionService != null && sourceAccount != null && destinationAccount != null)
                    {
                        var existingTransactions = await _transactionService.GetTransactionsAsync();
                        var matchingExistingTransactions = existingTransactions
                            .Where(t => t.FromAccountId == sourceAccount.Id && t.ToAccountId == destinationAccount.Id);

                        if (matchingExistingTransactions.Any())
                            isRecordAlreadyExists = true;

                        var matchingNewTransactions = transactions
                            .Where(t => t.FromAccountId == sourceAccount.Id && t.ToAccountId == destinationAccount.Id);

                        if (matchingNewTransactions.Any())
                            isRecordAlreadyExists = true;
                    }
                }

                if (transferredAccount != null)
                {
                    var transactionByAccount = new TransactionByAccount
                    {
                        SourceAccountId = amountValue > 0 ? (transferredAccount?.Id ?? 0) :(account?.Id ?? 0),
                        DestinationAccountId = amountValue > 0 ? (account?.Id ?? 0): (transferredAccount?.Id ?? 0),
                        TransactionDate = transactionDate,
                        Amount = amountValue,
                        Type = amountValue > 0 ? TransactionType.Income.ToString() : TransactionType.Expense.ToString(),
                        Reason = description,
                        FromAccount = account,
                        ToAccount = transferredAccount,
                    };
                    transactionByAccounts.Add(transactionByAccount);
                }
                else
                {
                    var transactionByCategory = new TransactionByCategory
                    {
                        CategoryId = category?.Id ?? 0,
                        TransactionDate = transactionDate,
                        Amount = amountValue,
                        Type = amountValue > 0 ? TransactionType.Income.ToString() : TransactionType.Expense.ToString(),
                        Reason = description,
                        Category = category,
                    };
                    transactionByCategorys.Add(transactionByCategory);
                }

                    var transaction = new Transaction
                    {
                        FromAccountId = account?.Id ?? 0,
                        ToAccountId = transferredAccount?.Id,
                        Category = category,
                        CategoryId = category?.Id,
                        Date = transactionDate,
                        Amount = amountValue,
                        Type = amountValue > 0 ? "Income" : "Expense",
                        Reason = description
                    };

                if (!isRecordAlreadyExists)
                {
                    transactions.Add(transaction);
                }
                else
                {
                    rejectedTransactions.Add(transaction);
                }
            }

            return transactions;
        }


        private async Task<Account?> DeriveAccountAsync(string accountField, string currency, Dictionary<string, Account> accountCache)
        {
            string? accName = null;

            if (accountField.Contains("To '"))
                accName = accountField.Split("To '").ElementAtOrDefault(1)?.Replace("'", "");
            else if (accountField.Contains("From '"))
                accName = accountField.Split("From '").ElementAtOrDefault(1)?.Replace("'", "");

            if (string.IsNullOrWhiteSpace(accName))
                return null;

            if (accountCache.TryGetValue(accName, out var account))
            {
                return account;
            }

            if (_accountService != null)
            {
                account = await _accountService.AddAccountAsync(new Account { Name = accName, Currency = currency });
                accountCache[accName] = account;
                return account;
            }

            return null;
        }


        #endregion Private Methods

        #region Public Methods
        public void Reset()
        {
            SelectedFilePath = null;
            CanUpload = false;
            IsUploading = false;
            UploadProgress = 0;
            UploadStatus = null;
        }
        #endregion Public Methods
    }

    public enum FileColumns
    {
        Date,
        AccountName,
        CategoryName,
        Amount,
        Currency,
        ConvertedCurrency,
        Description
    }
}
