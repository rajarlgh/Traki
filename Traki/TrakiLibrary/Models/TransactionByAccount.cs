using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TrakiLibrary.Models
{
    public class TransactionByAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }  // DB-generated primary key

        [ForeignKey(typeof(Account))]  // Foreign key linking to Account (FromAccount)
        public required int FromAccountId { get; set; }  // must link to account

        [ForeignKey(typeof(Account))]  // Foreign key linking to Account (ToAccount)
        public required int ToAccountId { get; set; }    // must link to account

        public required decimal Amount { get; set; }     // must provide amount
        public required string Type { get; set; }        // must provide type ("Income"/"Expense")

        public string? Reason { get; set; }              // optional reason

        public DateTime Date { get; set; }               // transaction date (no default, must set explicitly)
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
