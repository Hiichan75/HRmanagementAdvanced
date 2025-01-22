using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRmanagementAdvanced.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; }

        // Soft delete flag
        public bool IsDeleted { get; set; } = false;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
