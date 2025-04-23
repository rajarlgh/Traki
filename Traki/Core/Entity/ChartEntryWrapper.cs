using Microcharts;

namespace Core.Entity
{
    public class ChartEntryWrapper
    {
        public ChartEntry? Entry { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName => Entry?.Label;
    }
}
