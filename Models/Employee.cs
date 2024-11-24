using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRmanagementAdvanced.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string ContactInfo { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        public int DepartmentID { get; set; } // Foreign key

        public bool IsDeleted { get; set; } = false; // For soft delete

        public Department Department { get; set; } // Navigation property

        public ICollection<Absence> Absences { get; set; } // Navigation property for Absences
    }
}
