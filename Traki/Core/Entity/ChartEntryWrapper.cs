using SkiaSharp;

namespace Core.Entity
{
    public class ChartEntryWrapper
    {
        public decimal Value { get; set; }
        public string? Label { get; set; }
        public string? ValueLabel { get; set; }
        public SKColor Color { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName => Label;
    }
}
