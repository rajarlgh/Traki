using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TrakiLibrary.Models
{
    public class TransactionByCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Account))]
        public int? SourceAccountId { get; set; } // Foreign key linking to Account

        [ForeignKey(typeof(Category))]
        public int? CategoryId { get; set; }

        public decimal Amount { get; set; }  // removed `required`
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string Type { get; set; }     // removed `required`
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public string? Reason { get; set; }

        public DateTime TransactionDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        public string CreatedBy { get; set; } = "User";
        public string? UpdatedBy { get; set; }

        [ManyToOne]
        public Category? Category { get; set; }

    }
}
