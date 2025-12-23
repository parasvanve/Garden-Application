
using Azure;
using Azure.Data.Tables;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace GardenBookingApp.Models
{
    public class GardenBooking : Azure.Data.Tables.ITableEntity
    {
        // Azure Table Storage properties
        public string PartitionKey { get; set; } // Year-Month (e.g., "2024-12")
        public string RowKey { get; set; } // Unique ID
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Booking properties
        [Required]
        [Display(Name = "Your Name")]
        public string CustomerName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [IgnoreDataMember]
        public DateTime StartDate { get; set; }


        [IgnoreDataMember]
        public DateTime EndDate { get; set; }

        [Display(Name = "Number of People")]
        public int NumberOfPeople { get; set; }

        [Display(Name = "Purpose")]
        public string Purpose { get; set; }

        public DateTime CreatedDate { get; set; }

        public GardenBooking()
        {
            // Initialize with default values to avoid null errors
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            ETag = ETag.All;
        }


        public void SetKeys()
        {
            PartitionKey = BookingDate.ToString("yyyy-MM");
            RowKey = Guid.NewGuid().ToString();

            // Convert BookingDate to UTC to avoid Azure error
            BookingDate = DateTime.SpecifyKind(BookingDate, DateTimeKind.Utc);
            CreatedDate = DateTime.UtcNow;
        }


    }
}