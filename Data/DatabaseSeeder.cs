using HRmanagementAdvanced.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRmanagementAdvanced.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure roles exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Dummy users
            var dummyUsers = new[]
            {
                new { Username = "test123", Password = "123Abc!" },
                new { Username = "test1234", Password = "123Abc!" },
                new { Username = "test12345", Password = "123Abc!" }
            };

            foreach (var dummyUser in dummyUsers)
            {
                if (await userManager.FindByNameAsync(dummyUser.Username) == null)
                {
                    var user = new IdentityUser
                    {
                        UserName = dummyUser.Username,
                        EmailConfirmed = true // Optional, won't affect login
                    };

                    var result = await userManager.CreateAsync(user, dummyUser.Password);

                    if (result.Succeeded)
                    {
                        // Assign role to user
                        if (await roleManager.RoleExistsAsync("User"))
                        {
                            await userManager.AddToRoleAsync(user, "User");
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to create user {dummyUser.Username}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PersonenDbContext>();

            // Seed Departments
            if (!context.Departments.Any())
            {
                var departments = new[]
                {
                    new Department { DepartmentName = "HR" },
                    new Department { DepartmentName = "IT" },
                    new Department { DepartmentName = "Finance" },
                    new Department { DepartmentName = "Marketing" },
                    new Department { DepartmentName = "Sales" },
                    new Department { DepartmentName = "Operations" },
                    new Department { DepartmentName = "Customer Service" },
                    new Department { DepartmentName = "R&D" },
                    new Department { DepartmentName = "Legal" },
                    new Department { DepartmentName = "Administration" }
                };
                context.Departments.AddRange(departments);
                await context.SaveChangesAsync();
            }

            // Seed Employees
            if (!context.Employees.Any())
            {
                var employees = new[]
                {
                    new Employee { Name = "Alice Johnson", ContactInfo = "alice.johnson@example.com", Salary = 60000, DepartmentID = 1 },
                    new Employee { Name = "Bob Smith", ContactInfo = "bob.smith@example.com", Salary = 70000, DepartmentID = 2 },
                    new Employee { Name = "Charlie Brown", ContactInfo = "charlie.brown@example.com", Salary = 80000, DepartmentID = 3 },
                    new Employee { Name = "David Wilson", ContactInfo = "david.wilson@example.com", Salary = 75000, DepartmentID = 4 },
                    new Employee { Name = "Eva Green", ContactInfo = "eva.green@example.com", Salary = 65000, DepartmentID = 5 },
                    new Employee { Name = "Frank White", ContactInfo = "frank.white@example.com", Salary = 72000, DepartmentID = 6 },
                    new Employee { Name = "Grace Black", ContactInfo = "grace.black@example.com", Salary = 68000, DepartmentID = 7 },
                    new Employee { Name = "Hannah Blue", ContactInfo = "hannah.blue@example.com", Salary = 71000, DepartmentID = 8 },
                    new Employee { Name = "Ian Red", ContactInfo = "ian.red@example.com", Salary = 69000, DepartmentID = 9 },
                    new Employee { Name = "Jane Yellow", ContactInfo = "jane.yellow@example.com", Salary = 73000, DepartmentID = 10 }
                };
                context.Employees.AddRange(employees);
                await context.SaveChangesAsync();
            }

            // Seed Absences
            if (!context.Absences.Any())
            {
                var absences = new[]
                {
                    new Absence { EmployeeID = 1, Date = DateTime.Now.AddDays(-10), Reason = "Sick leave" },
                    new Absence { EmployeeID = 2, Date = DateTime.Now.AddDays(-8), Reason = "Vacation" },
                    new Absence { EmployeeID = 3, Date = DateTime.Now.AddDays(-6), Reason = "Personal leave" },
                    new Absence { EmployeeID = 4, Date = DateTime.Now.AddDays(-4), Reason = "Sick leave" },
                    new Absence { EmployeeID = 5, Date = DateTime.Now.AddDays(-2), Reason = "Vacation" },
                    new Absence { EmployeeID = 6, Date = DateTime.Now.AddDays(-1), Reason = "Personal leave" },
                    new Absence { EmployeeID = 7, Date = DateTime.Now.AddDays(-3), Reason = "Sick leave" },
                    new Absence { EmployeeID = 8, Date = DateTime.Now.AddDays(-5), Reason = "Vacation" },
                    new Absence { EmployeeID = 9, Date = DateTime.Now.AddDays(-7), Reason = "Personal leave" },
                    new Absence { EmployeeID = 10, Date = DateTime.Now.AddDays(-9), Reason = "Sick leave" }
                };
                context.Absences.AddRange(absences);
                await context.SaveChangesAsync();
            }
        }
    }
}
//