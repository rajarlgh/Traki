using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Core.Shared
{
    public class FilterChangedMessage : ValueChangedMessage<FilterState>
    {
        public FilterChangedMessage(FilterState value) : base(value) { }
    }

}
