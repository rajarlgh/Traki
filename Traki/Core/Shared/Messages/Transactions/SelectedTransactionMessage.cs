using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Core.Shared.Messages.Transactions
{
    public class SelectedTransactionMessage : ValueChangedMessage<TransactionMessage?>
    {
        public SelectedTransactionMessage(TransactionMessage? value = null) : base(value)
        {
        }
    }
}
