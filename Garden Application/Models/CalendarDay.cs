namespace Garden_Application.Models
{


        public class CalendarDay
        {
            public DateTime Date { get; set; }
            public bool IsBooked { get; set; }
            public string CustomerName { get; set; } = "";
           public string Purpose { get; set; } = "";
           public int NumberOfPeople { get; set; } = 0;
        }

       
}
