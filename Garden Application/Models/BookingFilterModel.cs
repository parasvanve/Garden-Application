namespace Garden_Application.Models
{
   
        public class BookingFilterModel
        {
            public string FilterType { get; set; }  // daily, weekly, monthly, custom
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    

}
