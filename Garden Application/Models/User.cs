using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace GardenBookingApp.Models
{
   
    public class User: ITableEntity
    {
        
        public string PartitionKey { get; set; } = default!;

        // RowKey: Must be unique within the Partition. For simple auth, this is the lowercased username.
        public string RowKey { get; set; } = default!;

        // --- User Data Properties ---

        [Required]
        public string Username { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        // Stores the securely HASHED password (e.g., using BCrypt).
        [Required]
        public string PasswordHash { get; set; } = default!;

        public string Role { get; set; } = "Customer";

        // --- Required ITableEntity Properties ---
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}