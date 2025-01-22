using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRmanagementAdvanced.Data;
using HRmanagementAdvanced.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize] // Restrict the entire controller to authenticated users
public class DepartmentsController : Controller
{
    private readonly PersonenDbContext _context;

    public DepartmentsController(PersonenDbContext context)
    {
        _context = context;
    }

    // Accessible by all authenticated users
    public async Task<IActionResult> Index(string sortOrder, string searchString)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

        var departmentsQuery = _context.Departments
            .Where(d => !d.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            departmentsQuery = departmentsQuery.Where(d => d.DepartmentName.Contains(searchString));
        }

        departmentsQuery = sortOrder switch
        {
            "name_desc" => departmentsQuery.OrderByDescending(d => d.DepartmentName),
            _ => departmentsQuery.OrderBy(d => d.DepartmentName),
        };

        return View(await departmentsQuery.ToListAsync());
    }



    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var department = await _context.Departments
            .FirstOrDefaultAsync(m => m.DepartmentID == id && !m.IsDeleted); // Exclude soft-deleted records
        if (department == null)
        {
            return NotFound();
        }

        return View(department);
    }

    // Restricted to Admins: Only Admins can manage departments
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("DepartmentID,DepartmentName")] Department department)
    {
        if (ModelState.IsValid)
        {
            _context.Add(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(department);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentID == id && !d.IsDeleted);
        if (department == null)
        {
            return NotFound();
        }
        return View(department);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,DepartmentName")] Department department)
    {
        if (id != department.DepartmentID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(department);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(department.DepartmentID))
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
        return View(department);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department != null)
        {
            department.IsDeleted = true; // Soft delete the record
            _context.Update(department);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }


    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department != null)
        {
            department.IsDeleted = true; // Mark as soft deleted
            _context.Update(department);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool DepartmentExists(int id)
    {
        return _context.Departments.Any(e => e.DepartmentID == id);
    }
    [HttpGet]
    public async Task<IActionResult> DepartmentDetailsAjax(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Employees) // Include employees if needed
            .FirstOrDefaultAsync(d => d.DepartmentID == id && !d.IsDeleted);

        if (department == null)
        {
            return NotFound();
        }

        return PartialView("_DepartmentDetailsPartial", department);
    }

}
