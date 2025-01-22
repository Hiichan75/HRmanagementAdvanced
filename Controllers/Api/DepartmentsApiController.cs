using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRmanagementAdvanced.Data;
using HRmanagementAdvanced.Models;

namespace HRmanagementAdvanced.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsApiController : ControllerBase
    {
        private readonly PersonenDbContext _context;

        public DepartmentsApiController(PersonenDbContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.Departments
                .Where(d => !d.IsDeleted)
                .ToListAsync();

            return Ok(departments);
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentID == id && !d.IsDeleted);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        // POST: api/Departments
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentID }, department);
        }

        // PUT: api/Departments/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] Department department)
        {
            if (id != department.DepartmentID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Departments/5 (Soft Delete)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            department.IsDeleted = true;
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(d => d.DepartmentID == id);
        }
    }
}
