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
    public class AbsencesApiController : ControllerBase
    {
        private readonly PersonenDbContext _context;

        public AbsencesApiController(PersonenDbContext context)
        {
            _context = context;
        }

        // GET: api/Absences
        [HttpGet]
        public async Task<IActionResult> GetAbsences()
        {
            var absences = await _context.Absences
                .Where(a => !a.IsDeleted)
                .Include(a => a.Employee)
                .ToListAsync();

            return Ok(absences);
        }

        // GET: api/Absences/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAbsence(int id)
        {
            var absence = await _context.Absences
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AbsenceID == id && !a.IsDeleted);

            if (absence == null)
            {
                return NotFound();
            }

            return Ok(absence);
        }

        // POST: api/Absences
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAbsence([FromBody] Absence absence)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Absences.Add(absence);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAbsence), new { id = absence.AbsenceID }, absence);
        }

        // PUT: api/Absences/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAbsence(int id, [FromBody] Absence absence)
        {
            if (id != absence.AbsenceID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(absence).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbsenceExists(id))
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

        // DELETE: api/Absences/5 (Soft Delete)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAbsence(int id)
        {
            var absence = await _context.Absences.FindAsync(id);
            if (absence == null)
            {
                return NotFound();
            }

            absence.IsDeleted = true;
            _context.Entry(absence).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AbsenceExists(int id)
        {
            return _context.Absences.Any(a => a.AbsenceID == id);
        }
    }
}
