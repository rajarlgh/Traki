using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Core.Shared
{
    public class AccountChangedMessage : ValueChangedMessage<AccountDetails>
    {
        public AccountChangedMessage(AccountDetails value) : base(value) { }
    }
}
