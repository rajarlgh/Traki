using CommunityToolkit.Mvvm.Messaging.Messages;
using Core.Entity;

namespace Core.Shared
{
    public class ShowTransactionsTabMessage : ValueChangedMessage<ChartEntryWrapper?>
    {
        public ShowTransactionsTabMessage(ChartEntryWrapper? value = null) : base(value)
        {
        }
    }
}
