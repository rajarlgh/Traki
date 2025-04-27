using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        #endregion Private Variables

        #region Observable Property
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

        #endregion Observable Property

        #region Constructor
        public ExcelUploaderViewModel(ITransactionService transactionService, IAccountService accountService, ICategoryService categoryService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
            _categoryService = categoryService;
            CanUpload = false; // Upload button is disabled initially
            _categoryService = categoryService;
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

        private async Task<List<Transaction>> ReadCsv(string filePath)
        {
            var transactions = new List<Transaction>();

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
                if (csv.GetField(0) == "date")
                    continue;

                //var date = DateTime.ParseExact(csv.GetField(0), "d/M/yyyy", CultureInfo.InvariantCulture);
                var date = DateTime.ParseExact(
                            csv.GetField(0) ?? throw new InvalidOperationException("Date field is missing."),
                            "d/M/yyyy",
                            CultureInfo.InvariantCulture);

                var accField = csv.GetField(1);
                var accCatField = csv.GetField(2);
                var amount = decimal.Parse(csv.GetField(3) ?? "0");
                var reason = csv.GetField(7);

                // Get or add account
                if (!existingAccounts.TryGetValue(accField ?? string.Empty, out var account))
                {
                    account = await _accountService.AddAccountAsync(new Account { Name = accField });
                    existingAccounts[accField ?? string.Empty] = account;
                }

                // Get or derive transferred account
                var transferredAccount = await DeriveAccountAsync(accCatField ?? string.Empty, existingAccounts);

                // Update account balance if needed
                if (accCatField != null && accCatField.Contains("Initial balance '"))
                {
                    account.InititalAccBalance = amount;
                    account.InitialAccDate = DateTime.Now;
                    await _accountService.UpdateAccountAsync(account);
                }

                Category? category = null;

                if (!string.IsNullOrWhiteSpace(accCatField) &&
                    !accCatField.Contains("From ") &&
                    !accCatField.Contains("To ") &&
                    !accCatField.Contains("Initial balance '"))
                {
                    if (!existingCategories.TryGetValue(accCatField, out category))
                    {
                        var categoryType = amount > 0 ? "Income" : "Expense";

                        category = await _categoryService.AddCategoryAsync(new Category
                        {
                            Name = accCatField,
                            Type = categoryType
                        });

                        existingCategories[accCatField] = category;
                    }
                }


                var transaction = new Transaction
                {
                    FromAccountId = account?.Id ?? 0,
                    ToAccountId = transferredAccount?.Id,
                    Category = category,
                    CategoryId = category?.Id,
                    Date = date,
                    Amount = amount,
                    Type = amount > 0 ? "Income" : "Expense",
                    Reason = reason
                };

                transactions.Add(transaction);
            }

            return transactions;
        }

        private async Task<Account?> DeriveAccountAsync(string accountField, Dictionary<string, Account> accountCache)
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
                account = await _accountService.AddAccountAsync(new Account { Name = accName });
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
}
