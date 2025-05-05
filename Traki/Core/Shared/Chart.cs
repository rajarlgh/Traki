using Core.Entity;
using Core.Enum;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using TrakiLibrary.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Shared
{
    public class Chart
    {
        private readonly Dictionary<string, SKColor> _categoryColors = new();
        private readonly List<SKColor> _availableColors = new()
        {
            SKColor.Parse("#00FF00"), // Green
            SKColor.Parse("#FF5733"), // Orange
            SKColor.Parse("#3498DB"), // Blue
            SKColor.Parse("#9B59B6"), // Purple
            SKColor.Parse("#1ABC9C"), // Teal
            SKColor.Parse("#F1C40F"), // Yellow
            SKColor.Parse("#E74C3C"), // Red
            SKColor.Parse("#34495E"), // Dark Gray
            SKColor.Parse("#2ECC71"), // Light Green
            SKColor.Parse("#E67E22"), // Light Orange
            SKColor.Parse("#16A085"), // Dark Teal
            SKColor.Parse("#8E44AD"), // Deep Purple
            SKColor.Parse("#BDC3C7")  // Light Gray
        };
        private int _colorIndex = 0;
        public SKColor GetCategoryColor(string category)
        {
            if (!_categoryColors.ContainsKey(category))
            {
                // Assign the next available color
                var color = _availableColors[_colorIndex];
                _categoryColors[category] = color;

                // Update the color index, wrap around if necessary
                _colorIndex = (_colorIndex + 1) % _availableColors.Count;
            }

            return _categoryColors[category];
        }

        public List<ChartEntryWrapper> CreateChart(TransactionFilterRequest accountDetails, TransactionType transactionType)
        {
            var result = new List<ChartEntryWrapper>();
            if (accountDetails == null)
                return result;

            var filterOption = accountDetails.FilterOption;
            var transactions = accountDetails.Transactions;
            var selectedAccountId = accountDetails.AccountId;
            var fromDate = accountDetails.FromDate;
            var toDate = accountDetails.ToDate;
            var categories = accountDetails.Categories;
            var accounts = accountDetails.Accounts;


            var filteredTransactions = new List<Transaction>();

            if (filterOption != FilterOption.All && transactions != null)
            {
                if (selectedAccountId > 0)
                {
                    filteredTransactions = transactions
                        .Where(t => t.Date >= fromDate && t.Date <= toDate &&
                                   (t.FromAccountId == selectedAccountId || t.ToAccountId == selectedAccountId))
                        .ToList();
                }
                else
                {
                    filteredTransactions = transactions
                        .Where(t => t.Date >= fromDate && t.Date <= toDate)
                        .ToList();
                }
            }
            else
            {
                filteredTransactions = transactions ?? new List<Transaction>();
            }

            if (filteredTransactions.Any() && categories != null)
            {
                IEnumerable<AggregatedTransaction> outgoing = Enumerable.Empty<AggregatedTransaction>();

                if (transactionType == TransactionType.Expense)
                {
                    foreach (var t in filteredTransactions)
                    {
                        if (t.Type == TransactionType.Income.ToString()
                            && (t.FromAccountId == 0? true: t.FromAccountId == selectedAccountId)
                            && t.FromAccountId > 0
                            && t.ToAccountId > 0)
                        {
                            t.Type = TransactionType.Expense.ToString(); // Correct it
                        }
                    }

                    // Scenario 1: Outgoing transactions (act as Expense for selected account)
                    outgoing = filteredTransactions
                    .Where(t => t.Type == transactionType.ToString()
                                && (t.FromAccountId == 0? true: t.FromAccountId == selectedAccountId)
                                && t.FromAccountId > 0
                                && t.ToAccountId > 0)
                    .GroupBy(t => t.ToAccountId)
                    .Select(g =>
                    {
                        var accountName = accounts?.FirstOrDefault(a => a.Id == g.Key)?.Name;
                        return new AggregatedTransaction
                        {
                            Id = g.Key,
                            Name = accountName,
                            TotalAmount = g.Sum(t => t.Amount * -1) // Subtract for sender
                        };
                    });
                    var t1 = outgoing.ToList();
                }
                IEnumerable<AggregatedTransaction> incoming = Enumerable.Empty<AggregatedTransaction>();

                if (transactionType == TransactionType.Income)
                {
                    // Scenario 2: Incoming transactions (act as Income for selected account)
                  incoming = filteredTransactions
                        .Where(t => t.Type == transactionType.ToString()
                                    && (t.FromAccountId == 0 ? true : t.ToAccountId == selectedAccountId)
                                    && t.FromAccountId > 0
                                    && t.ToAccountId > 0)
                        .GroupBy(t => t.FromAccountId)
                        .Select(g =>
                        {
                            var accountName = accounts?.FirstOrDefault(a => a.Id == g.Key)?.Name;
                            return new AggregatedTransaction
                            {
                                Id = g.Key,
                                Name = accountName,
                                TotalAmount = g.Sum(t => t.Amount) // Add for receiver
                            };
                        });
                    var t2 = incoming.ToList();
                }
                // Group by Category as before (for non-transfer transactions)
                var categoryGroups = filteredTransactions
                    .Where(t => t.FromAccountId == 0 ? true : t.FromAccountId == selectedAccountId && t.Type == transactionType.ToString() && t.CategoryId != null)
                    .GroupBy(t => t.CategoryId)
                    .Select(g =>
                    {
                        var categoryName = categories.FirstOrDefault(c => c.Id == g.Key)?.Name;
                        return new AggregatedTransaction
                        {
                            Id = g.Key,
                            Name = categoryName,
                            TotalAmount = g.Sum(t => t.Amount)
                        };
                    });
                var t3 = categoryGroups.ToList();

                //categoryGroups.ToList().AddRange(outgoing.ToList());
                //categoryGroups.ToList().AddRange(incoming.ToList());
                // Merge all results
                var merged = categoryGroups
                    .Concat(outgoing)
                    .Concat(incoming)
                    .ToList();

                result = merged.Select(data => new ChartEntryWrapper
                {
                    Label = data.Name,
                    ValueLabel = data.TotalAmount.ToString("F0"),
                    Value = (float)data.TotalAmount,
                    Color = this.GetCategoryColor(data.Name ?? string.Empty),
                    CategoryId = data.Id
                }).ToList();
            }

            return result;
        }
    }
    public class AggregatedTransaction
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
