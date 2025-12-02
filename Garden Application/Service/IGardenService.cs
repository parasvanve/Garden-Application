using Garden_Application.Models;
using GardenBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GardenBookingApp.Services
{
    public interface IGardenService
    {
        Task<bool> CreateBookingAsync(GardenBooking booking);
        Task<List<GardenBooking>> GetAllBookingsAsync();
        Task<List<GardenBooking>> GetUpcomingBookingsAsync();
        Task<bool> IsDateBookedAsync(DateTime date);
        Task<List<DateTime>> GetBookedDatesAsync();
        Task<GardenBooking> GetBookingByIdAsync(string partitionKey, string rowKey);
        Task<bool> UpdateBookingAsync(GardenBooking booking);
        Task<bool> DeleteBookingAsync(string partitionKey, string rowKey);

        // FILTER (Correct return type)
        Task<List<GardenBooking>> GetFilteredBookingsAsync(BookingFilterModel filter);
    }
}
