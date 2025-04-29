using Core.Entity;
using Core.Enum;
using SkiaSharp;
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
            var accountId = accountDetails.AccountId;
            var fromDate = accountDetails.FromDate;
            var toDate = accountDetails.ToDate;
            var categories = accountDetails.Categories;

            var filteredTransactions = new List<Transaction>();

            if (filterOption != FilterOption.All && transactions != null)
            {
                if (accountId > 0)
                {
                    filteredTransactions = transactions
                        .Where(t => t.Date >= fromDate && t.Date <= toDate && t.AccountId == accountId)
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
            { filteredTransactions = transactions; }

            if (filteredTransactions != null && categories != null)
            {
                var groupedData = filteredTransactions
                    .Where(t => t.Type == transactionType.ToString() && t.CategoryId != null)
                    .GroupBy(t => t.CategoryId)
                    .Select(g =>
                    {
                        var categoryId = g.Key;
                        var categoryName = categories.FirstOrDefault(c => c.Id == categoryId)?.Name;

                        return new
                        {
                            CategoryId = categoryId,
                            CategoryName = categoryName,
                            TotalAmount = g.Sum(t => t.Amount)
                        };
                    })
                    .ToList();

                var t = groupedData.Count();
                result = groupedData.Select(data => new ChartEntryWrapper
                {

                    Label = data.CategoryName,
                    ValueLabel = data.TotalAmount.ToString("F0"),
                    Value = (float)data.TotalAmount, // 👈 SET THIS
                    Color = this.GetCategoryColor(data.CategoryName ?? string.Empty),
                    CategoryId = data.CategoryId,
                }).ToList();
            }
            return result;
        }
    }
}
