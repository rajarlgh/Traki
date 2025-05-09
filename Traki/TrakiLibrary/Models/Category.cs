using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TrakiLibrary.Models
{
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Type { get; set; }

        // One-to-Many relationship: A Category can have many transactions
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TransactionByCategory> Transactions { get; set; } = new List<TransactionByCategory>();
    }
}
