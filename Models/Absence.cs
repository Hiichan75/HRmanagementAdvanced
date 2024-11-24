using System;
using System.ComponentModel.DataAnnotations;

namespace HRmanagementAdvanced.Models
{
    public class Absence
    {
        public int AbsenceID { get; set; }

        public int EmployeeID { get; set; } // Foreign key

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        public Employee Employee { get; set; } // Navigation property
    }
}
