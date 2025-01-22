using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HRmanagementAdvanced.Models;

namespace HRmanagementAdvanced.Data
{
    public class PersonenDbContext : IdentityDbContext<CustomUser>
    {
        public PersonenDbContext(DbContextOptions<PersonenDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Absence> Absences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentID);

            // Configure decimal precision for the Salary property
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Salary)
                      .HasColumnType("decimal(18, 2)"); // Explicitly set precision and scale
            });

            modelBuilder.Entity<Absence>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Absences)
                .HasForeignKey(a => a.EmployeeID);

            modelBuilder.Entity<CustomUser>()
                .Property(u => u.FirstName)
                .HasMaxLength(50);

            modelBuilder.Entity<CustomUser>()
                .Property(u => u.LastName)
                .HasMaxLength(50);



        }
    }
}
