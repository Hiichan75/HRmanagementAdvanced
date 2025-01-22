using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRmanagementAdvanced.Data;
using HRmanagementAdvanced.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class EmployeesController : Controller
{
    private readonly PersonenDbContext _context;

    public EmployeesController(PersonenDbContext context)
    {
        _context = context;
    }

    // Display all non-deleted employees
    public async Task<IActionResult> Index(string sortOrder, string searchString)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DepartmentSortParm"] = sortOrder == "Department" ? "department_desc" : "Department";

        var employeesQuery = _context.Employees
            .Where(e => !e.IsDeleted)
            .Include(e => e.Department)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            employeesQuery = employeesQuery.Where(e => e.Name.Contains(searchString) ||
                                                       e.ContactInfo.Contains(searchString));
        }

        employeesQuery = sortOrder switch
        {
            "name_desc" => employeesQuery.OrderByDescending(e => e.Name),
            "Department" => employeesQuery.OrderBy(e => e.Department.DepartmentName),
            "department_desc" => employeesQuery.OrderByDescending(e => e.Department.DepartmentName),
            _ => employeesQuery.OrderBy(e => e.Name),
        };

        return View(await employeesQuery.ToListAsync());
    }



    // View employee details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
    .Include(e => e.Absences)
    .Include(e => e.Department)
    .FirstOrDefaultAsync(e => e.EmployeeID == id && !e.IsDeleted);
        // Exclude deleted employees

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // Create a new employee
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        // Load non-deleted departments for the dropdown
        ViewData["DepartmentID"] = new SelectList(
            _context.Departments.Where(d => !d.IsDeleted),
            "DepartmentID",
            "DepartmentName");

        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,ContactInfo,Salary,DepartmentID")] Employee employee)
    {
        if (ModelState.IsValid)
        {

            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Redirect to the list after creation
        }

        // Reload departments in case of an error
        ViewData["DepartmentID"] = new SelectList(
            _context.Departments.Where(d => !d.IsDeleted),
            "DepartmentID",
            "DepartmentName",
            employee.DepartmentID);

        return View(employee);
    }

    // Edit an existing employee
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeID == id && !e.IsDeleted);
        if (employee == null)
        {
            return NotFound();
        }

        ViewData["DepartmentID"] = new SelectList(_context.Departments.Where(d => !d.IsDeleted), "DepartmentID", "DepartmentName", employee.DepartmentID);
        return View(employee);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,Name,ContactInfo,Salary,DepartmentID")] Employee employee)
    {
        if (id != employee.EmployeeID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Employees.Any(e => e.EmployeeID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["DepartmentID"] = new SelectList(_context.Departments.Where(d => !d.IsDeleted), "DepartmentID", "DepartmentName", employee.DepartmentID);
        return View(employee);
    }


    // Soft delete an employee
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            employee.IsDeleted = true; // Mark as soft-deleted
            _context.Update(employee);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.EmployeeID == id);
    }
    // Ajax method for dynamic employee details
    [HttpGet]
    public async Task<IActionResult> EmployeeDetailsAjax(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Absences)
            .FirstOrDefaultAsync(e => e.EmployeeID == id && !e.IsDeleted);

        if (employee == null)
        {
            return NotFound();
        }

        return PartialView("_EmployeeDetailsPartial", employee); // Ensure this matches the partial view name
    }

}

