﻿using Core.Enum;
using TrakiLibrary.Models;

namespace Core.Shared
{
    public class TransactionFilterRequest
    {
        public List<TransactionByCategory>? TransactionByCategorys { get; set; }
        public List<TransactionByAccount>? TransactionByAccounts { get; set; }
        public FilterOption? FilterOption{ get; set; }
        public int AccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; } = DateTime.Now;
        public List<Category>? Categories { get; set; }
        public SkiaSharp.SKColor SKColor { get; set; } 
        public List<Account>? Accounts { get; set; }
    }
}
