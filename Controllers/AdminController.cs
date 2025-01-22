using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HRmanagementAdvanced.Models;

[Authorize(Roles = "Admin")] // Restrict access to Admins only
public class AdminController : Controller
{
    private readonly UserManager<CustomUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Display a list of users with their roles
    public async Task<IActionResult> ManageUsers()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new Dictionary<string, IList<string>>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(user.Id, roles);
        }

        ViewData["UserRoles"] = userRoles;
        return View(users);
    }

    // Assign a role to a user
    [HttpPost]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _roleManager.RoleExistsAsync(role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove user from all current roles
            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Add the user to the new role
            await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction("ManageUsers");
    }

    // Delete a user permanently
    [HttpPost]
    public async Task<IActionResult> HardDeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction("ManageUsers");
    }
}
