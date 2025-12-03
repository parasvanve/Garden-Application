using Garden_Application.Models;
using GardenBookingApp.Models;
using GardenBookingApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq; // Added for LINQ methods
using System.Threading.Tasks;

namespace GardenBookingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGardenService _gardenService;

        public HomeController(IGardenService gardenService)
        {
            _gardenService = gardenService;
        }

        public async Task<IActionResult> Index(string selectedDate)
        {
            var bookedDates = await _gardenService.GetBookedDatesAsync();
            ViewBag.BookedDates = bookedDates;

            // Pass selected date to view if provided from calendar
            if (!string.IsNullOrEmpty(selectedDate))
            {
                ViewBag.SelectedDate = selectedDate;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Recommended for POST actions
        public async Task<IActionResult> Book(GardenBooking booking)
        {
            if (ModelState.IsValid)
            {
                // Check if date is already booked
                var isBooked = await _gardenService.IsDateBookedAsync(booking.BookingDate);

                if (isBooked)
                {
                    TempData["Error"] = "Sorry! Garden is already booked for this date.";
                    return RedirectToAction("Index");
                }

                var success = await _gardenService.CreateBookingAsync(booking);

                if (success)
                {
                    TempData["Success"] = $"Garden booked successfully for {booking.BookingDate.ToString("dd MMM yyyy")}!";
                }
                else
                {
                    TempData["Error"] = "Booking failed. Please try again.";
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MyBookings()
        {
            var bookings = await _gardenService.GetUpcomingBookingsAsync();
            return View(bookings);
        }

        public async Task<IActionResult> AllBookings(string filter = "all", DateTime? startDate = null, DateTime? endDate = null)
        {
            var bookings = await _gardenService.GetAllBookingsAsync();

            // Apply filters
            switch (filter?.ToLower())
            {
                case "daily":
                    bookings = bookings
                        .Where(b => b.BookingDate.Date == DateTime.Today)
                        .ToList();
                    break;

                case "weekly":
                    var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
                    var endOfWeek = startOfWeek.AddDays(6);
                    bookings = bookings
                        .Where(b => b.BookingDate.Date >= startOfWeek && b.BookingDate <= endOfWeek)
                        .ToList();
                    break;

                case "monthly":
                    bookings = bookings
                        .Where(b => b.BookingDate.Month == DateTime.Today.Month &&
                                    b.BookingDate.Year == DateTime.Today.Year)
                        .ToList();
                    break;

                case "custom":
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        bookings = bookings
                            .Where(b => b.BookingDate.Date >= startDate.Value.Date &&
                                        b.BookingDate.Date <= endDate.Value.Date)
                            .ToList();
                    }
                    break;
            }

            return View(bookings);
        }


        [HttpGet]
        public async Task<IActionResult> Calendar()
        {
            var bookedDates = await _gardenService.GetBookedDatesAsync();
            ViewBag.BookedDates = bookedDates;
            return View();
        }

        public async Task<IActionResult> PublicCalendar()
        {
            var bookedDates = await _gardenService.GetBookedDatesAsync();
            ViewBag.BookedDates = bookedDates;
            return View();
        }




        [HttpGet]
        public async Task<IActionResult> GetBookingsForMonth(int year, int month)
        {
            var bookings = await _gardenService.GetAllBookingsAsync();
            var monthBookings = bookings
                .Where(b => b.BookingDate.Year == year && b.BookingDate.Month == month)
                .Select(b => new
                {
                    date = b.BookingDate.ToString("yyyy-MM-dd"),
                    name = b.CustomerName,
                    people = b.NumberOfPeople,
                    purpose = b.Purpose
                })
                .ToList();

            return Json(monthBookings);
        }

        // --- FIXED: [HttpGet] Edit Action (Uncommented) ---
        // GET: /Home/Edit?partitionKey={pk}&rowKey={rk} (Shows the Edit form)
        [HttpGet]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                TempData["Error"] = "Invalid booking reference provided for editing.";
                return RedirectToAction(nameof(AllBookings));
            }

            // Assumes this method exists in your service
            var booking = await _gardenService.GetBookingByIdAsync(partitionKey, rowKey);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction(nameof(AllBookings));
            }

            return View(booking);
        }

        // POST: /Home/Edit (Processes the Edit form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GardenBooking booking)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(booking.PartitionKey) || string.IsNullOrEmpty(booking.RowKey))
                {
                    TempData["Error"] = "Missing booking key information. Cannot update.";
                    return RedirectToAction(nameof(AllBookings));
                }

                var success = await _gardenService.UpdateBookingAsync(booking);

                if (success)
                {
                    TempData["Success"] = $"Booking for {booking.CustomerName} updated successfully!";
                    return RedirectToAction(nameof(AllBookings));
                }
                else
                {
                    TempData["Error"] = "Failed to update booking. It may have been modified concurrently. Please try again.";
                }
            }

            // If ModelState is invalid or update failed, return the view to display errors/error message
            return View(booking);
        }

        // --- FIXED: [HttpGet] Delete Action (Uncommented) ---
        // GET: /Home/Delete?partitionKey={pk}&rowKey={rk} (Shows the Confirmation screen)
        [HttpGet]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                TempData["Error"] = "Invalid booking reference provided for deletion.";
                return RedirectToAction(nameof(AllBookings));
            }

            // Assumes this method exists in your service
            var booking = await _gardenService.GetBookingByIdAsync(partitionKey, rowKey);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction(nameof(AllBookings));
            }

            return View(booking);
        }

        // POST: /Home/Delete (Performs the deletion)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                TempData["Error"] = "Missing booking key information. Cannot delete.";
                return RedirectToAction(nameof(AllBookings));
            }

            var success = await _gardenService.DeleteBookingAsync(partitionKey, rowKey);

            if (success)
            {
                TempData["Success"] = "Booking deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete booking. Please try again.";
            }

            return RedirectToAction(nameof(AllBookings));
        }
    
    }
}