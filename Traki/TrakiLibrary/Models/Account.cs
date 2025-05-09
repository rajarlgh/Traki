using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TrakiLibrary.Models
{
    public class Account
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public required string Name { get; set; } // e.g., "Savings Account"
        public required string Currency { get; set; } // e.g., "USD", "EUR"

        public decimal? InititalAccBalance { get; set; }
        public DateTime InitialAccDate { get; set; } = DateTime.Now;

        // One-to-Many relationship: An Account can have many transactions
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TransactionByAccount> Transactions { get; set; } = new List<TransactionByAccount>();
    }
}
