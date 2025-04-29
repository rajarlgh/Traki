using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Core.Shared
{
    public class AccountChangedMessage : ValueChangedMessage<TransactionFilterRequest>
    {
        public AccountChangedMessage(TransactionFilterRequest value) : base(value) { }
    }
}
