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

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
