using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Core.Shared
{
    public class ShowTransactionsTabMessage : ValueChangedMessage<object?>
    {
        public ShowTransactionsTabMessage(object? value = null) : base(value)
        {
        }
    }
}
