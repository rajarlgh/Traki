using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TrakiLibrary.Models
{
    public class TransactionByCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }  // DB-generated primary key

        [ForeignKey(typeof(Category))]  // Foreign key linking to Account (FromAccount)
        public int? CategoryId { get; set; }             // Foreign key linking to Category
        public required decimal Amount { get; set; }     // must provide amount
        public required string Type { get; set; }        // must provide type ("Income"/"Expense")

        public string? Reason { get; set; }              // optional reason

        public DateTime Date { get; set; }               // transaction date (no default, must set explicitly)
        public DateTime CreatedDate { get; set; } = DateTime.Now;  // defaults to now
        public DateTime? UpdatedDate { get; set; }                   // optional updated date

        public string CreatedBy { get; set; } = "User";             // defaults to "User"
        public string? UpdatedBy { get; set; }                      // optional updated by

        // Navigation property to the Category (optional, but useful for loading the associated category)
        [ManyToOne]
        public Category? Category { get; set; }  // navigation to Category
    }
}
