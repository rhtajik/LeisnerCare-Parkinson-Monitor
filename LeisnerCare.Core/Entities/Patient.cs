using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeisnerCare.Core.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Link til ApplicationUser
        public string CprNumber { get; set; } = string.Empty; // 10 cifre
        public DateTime DateOfBirth { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string? ContactPhone { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser? User { get; set; }
    }
}
