using Core.Enum;
using TrakiLibrary.Models;

namespace Core.Shared.Messages.Transactions
{
    public class TransactionMessage
    {
        public int CategoryId { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
