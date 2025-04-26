using Core.Enum;
using TrakiLibrary.Models;

namespace Core.Shared
{
    public class FilterState
    {
        public FilterOption? SelectedFilterOption { get; set; }
        public string? SelectedMonth { get; set; }
        public string? SelectedWeek { get; set; }
        public int? SelectedYear { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime OnDate { get; set; }
        public Account? SelectedAccount { get; set; }
    }
}
