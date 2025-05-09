using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TrakiLibrary.Models
{
    public class TransactionByAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }  // DB-generated primary key

        [ForeignKey(typeof(Account))]  // Foreign key linking to Account (FromAccount)
        public int SourceAccountId { get; set; }  // must link to account

        [ForeignKey(typeof(Account))]  // Foreign key linking to Account (ToAccount)
        public int DestinationAccountId { get; set; }    // must link to account

        public decimal Amount { get; set; }     // must provide amount
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string Type { get; set; }        // must provide type ("Income"/"Expense")
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public string? Reason { get; set; }              // optional reason

        public DateTime TransactionDate { get; set; }               // transaction date (no default, must set explicitly)
        public DateTime CreatedDate { get; set; } = DateTime.Now;  // defaults to now
        public DateTime? UpdatedDate { get; set; }                   // optional updated date

        public string CreatedBy { get; set; } = "User";             // defaults to "User"
        public string? UpdatedBy { get; set; }                      // optional updated by

        // Navigation properties (optional)
        [ManyToOne]
        public Account? FromAccount { get; set; }  // Navigation to the source account

        [ManyToOne]
        public Account? ToAccount { get; set; }    // Navigation to the destination account

    }
}
