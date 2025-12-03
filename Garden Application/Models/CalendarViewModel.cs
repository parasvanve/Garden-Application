namespace Garden_Application.Models
{
    public class CalendarViewModel
    {
        public string MonthName { get; set; }
        public int Year { get; set; }
        public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();
    }

}
