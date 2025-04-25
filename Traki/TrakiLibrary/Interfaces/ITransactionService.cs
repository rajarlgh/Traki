﻿using TrakiLibrary.Models;

namespace TrakiLibrary.Interfaces
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(int id);
        Task UpdateTransactionAsync(Transaction transaction);
    }
}
