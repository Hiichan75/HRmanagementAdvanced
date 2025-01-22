using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRmanagementAdvanced.Data;
using HRmanagementAdvanced.Models;

[Authorize]
public class AbsencesController : Controller
{
    private readonly PersonenDbContext _context;

    public AbsencesController(PersonenDbContext context)
    {
        _context = context;
    }

    // Accessible by all authenticated users
    public async Task<IActionResult> Index(string sortOrder, string searchString)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
        ViewData["EmployeeSortParm"] = sortOrder == "Employee" ? "employee_desc" : "Employee";

        var absencesQuery = _context.Absences
            .Where(a => !a.IsDeleted)
            .Include(a => a.Employee)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            absencesQuery = absencesQuery.Where(a => a.Reason.Contains(searchString) ||
                                                     a.Employee.Name.Contains(searchString));
        }

        absencesQuery = sortOrder switch
        {
            "date_desc" => absencesQuery.OrderByDescending(a => a.Date),
            "Employee" => absencesQuery.OrderBy(a => a.Employee.Name),
            "employee_desc" => absencesQuery.OrderByDescending(a => a.Employee.Name),
            _ => absencesQuery.OrderBy(a => a.Date),
        };

        return View(await absencesQuery.ToListAsync());
    }



    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var absence = await _context.Absences
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(m => m.AbsenceID == id && !m.IsDeleted); // Exclude soft-deleted records

        if (absence == null)
        {
            return NotFound();
        }

        return View(absence);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "Name");
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AbsenceID,EmployeeID,Date,Reason")] Absence absence)
    {
        if (ModelState.IsValid)
        {
            _context.Add(absence);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "Name", absence.EmployeeID);
        return View(absence);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var absence = await _context.Absences
            .FirstOrDefaultAsync(a => a.AbsenceID == id && !a.IsDeleted); // Exclude soft-deleted records

        if (absence == null)
        {
            return NotFound();
        }

        ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "Name", absence.EmployeeID);
        return View(absence);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("AbsenceID,EmployeeID,Date,Reason")] Absence absence)
    {
        if (id != absence.AbsenceID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(absence);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbsenceExists(absence.AbsenceID))
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
        ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "Name", absence.EmployeeID);
        return View(absence);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var absence = await _context.Absences.FindAsync(id);
        if (absence != null)
        {
            absence.IsDeleted = true; // Soft delete the record
            _context.Update(absence);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var absence = await _context.Absences.FindAsync(id);
        if (absence != null)
        {
            absence.IsDeleted = true; // Soft delete
            _context.Update(absence);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AbsenceExists(int id)
    {
        return _context.Absences.Any(e => e.AbsenceID == id && !e.IsDeleted);
    }
}
