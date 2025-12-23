using Azure;
using Azure.Data.Tables;
using Garden_Application.Models;
using GardenBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GardenBookingApp.Services
{
    public class GardenService : IGardenService
    {
        private readonly TableClient _tableClient;

        public GardenService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureTableStorage");
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient("GardenBookings");
            _tableClient.CreateIfNotExists();
        }

        public async Task<bool> CreateBookingAsync(GardenBooking booking)
        {
            try
            {
                // 🔴 VERY IMPORTANT FIX
                booking.BookingDate = DateTime.SpecifyKind(booking.BookingDate, DateTimeKind.Utc);

                booking.CreatedDate = DateTime.SpecifyKind(
                    booking.CreatedDate == default ? DateTime.UtcNow : booking.CreatedDate,
                    DateTimeKind.Utc
                );

                booking.SetKeys();
                await _tableClient.AddEntityAsync(booking);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<List<GardenBooking>> GetAllBookingsAsync()
        {
            var bookings = new List<GardenBooking>();

            await foreach (var booking in _tableClient.QueryAsync<GardenBooking>())
            {
                bookings.Add(booking);
            }

            return bookings.OrderBy(b => b.BookingDate).ToList();
        }

        public async Task<List<GardenBooking>> GetUpcomingBookingsAsync()
        {
            var bookings = new List<GardenBooking>();
            var today = DateTime.Today;

            await foreach (var booking in _tableClient.QueryAsync<GardenBooking>())
            {
                if (booking.BookingDate >= today)
                {
                    bookings.Add(booking);
                }
            }

            return bookings.OrderBy(b => b.BookingDate).ToList();
        }

        public async Task<bool> IsDateBookedAsync(DateTime date)
        {
            await foreach (var booking in _tableClient.QueryAsync<GardenBooking>())
            {
                if (booking.BookingDate.Date == date.Date)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<List<DateTime>> GetBookedDatesAsync()
        {
            var dates = new List<DateTime>();
            var today = DateTime.Today;

            await foreach (var booking in _tableClient.QueryAsync<GardenBooking>())
            {
                if (booking.BookingDate >= today)
                {
                    dates.Add(booking.BookingDate.Date);
                }
            }

            return dates.Distinct().OrderBy(d => d).ToList();
        }

        public async Task<GardenBooking> GetBookingByIdAsync(string partitionKey, string rowKey)
        {
            try
            {
                Response<GardenBooking> response =
                    await _tableClient.GetEntityAsync<GardenBooking>(partitionKey, rowKey);

                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateBookingAsync(GardenBooking booking)
        {
            try
            {
                if (booking.BookingDate.Kind == DateTimeKind.Unspecified)
                {
                    booking.BookingDate = DateTime.SpecifyKind(booking.BookingDate, DateTimeKind.Utc);
                }

                if (booking.CreatedDate.Kind == DateTimeKind.Unspecified)
                {
                    booking.CreatedDate = DateTime.SpecifyKind(booking.CreatedDate, DateTimeKind.Utc);
                }

                await _tableClient.UpdateEntityAsync(booking, booking.ETag, TableUpdateMode.Replace);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 412)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteBookingAsync(string partitionKey, string rowKey)
        {
            try
            {
                await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ⭐ CORRECTED FILTER METHOD
        public async Task<List<GardenBooking>> GetFilteredBookingsAsync(BookingFilterModel filter)
        {
            var bookings = new List<GardenBooking>();
            await foreach (var booking in _tableClient.QueryAsync<GardenBooking>())
            {
                bookings.Add(booking);
            }

            DateTime today = DateTime.UtcNow.Date;

            if (filter.FilterType == "daily")
            {
                bookings = bookings.Where(b => b.BookingDate.Date == today).ToList();
            }
            else if (filter.FilterType == "weekly")
            {
                int diff = (int)today.DayOfWeek - 1;
                if (diff < 0) diff = 6;

                var start = today.AddDays(-diff);
                var end = start.AddDays(6);

                bookings = bookings.Where(b =>
                    b.BookingDate >= start && b.BookingDate <= end).ToList();
            }
            else if (filter.FilterType == "monthly")
            {
                var start = new DateTime(today.Year, today.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);

                bookings = bookings.Where(b =>
                    b.BookingDate >= start && b.BookingDate <= end).ToList();
            }
            else if (filter.FilterType == "custom" &&
                     filter.StartDate.HasValue &&
                     filter.EndDate.HasValue)
            {
                var start = filter.StartDate.Value.Date;
                var end = filter.EndDate.Value.Date;

                bookings = bookings.Where(b =>
                    b.BookingDate.Date >= start &&
                    b.BookingDate.Date <= end).ToList();
            }

            return bookings.OrderByDescending(b => b.BookingDate).ToList();
        }
    }
}
