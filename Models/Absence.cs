using System;
using System.ComponentModel.DataAnnotations;

namespace HRmanagementAdvanced.Models
{
    public class Absence
    {
        public int AbsenceID { get; set; }

        [Required]
        public int EmployeeID { get; set; } // Foreign key

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        public bool IsDeleted { get; set; } = false; // For soft delete

        public Employee Employee { get; set; } // Navigation property
    }
}