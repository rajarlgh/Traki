using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TrakiLibrary.Models
{
    public class Account
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string Name { get; set; } // e.g., "Savings Account"
        public string Currency { get; set; } // e.g., "USD", "EUR"
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public decimal? InititalAccBalance { get; set; }
        public DateTime InitialAccDate { get; set; } = DateTime.Now;

        // One-to-Many relationship: An Account can have many transactions
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TransactionByAccount> Transactions { get; set; } = new List<TransactionByAccount>();
    }
}
