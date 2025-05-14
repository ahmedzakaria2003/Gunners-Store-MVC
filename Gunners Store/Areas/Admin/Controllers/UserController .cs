using Gunner.DataAccess.Data;
using Gunner.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public UserController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var usersWithRoles = _dbContext.ApplicationUsers
            .Select(user => new
            {
                id = user.Id,
                name = user.UserName,
                email = user.Email,
                lockoutEnd = user.LockoutEnd,
                role = _dbContext.UserRoles
                    .Join(_dbContext.Roles,
                        userRole => userRole.RoleId,
                        role => role.Id,
                        (userRole, role) => new { userRole, role })
                    .Where(ur => ur.userRole.UserId == user.Id)
                    .Select(ur => ur.role.Name)
                    .FirstOrDefault() ?? "No Role"
            })
            .ToList();

        return Json(new { data = usersWithRoles });
    }

    [HttpPost]
    public IActionResult ToggleLock([FromBody] string id)
    {
        var user = _dbContext.ApplicationUsers.Find(id);
        if (user == null)
        {
            return Json(new { success = false, message = "User not found" });
        }

        if (user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now)
        {
            user.LockoutEnd = DateTimeOffset.Now.AddYears(1000); // Lock
        }
        else
        {
            user.LockoutEnd = null; // Unlock
        }

        _dbContext.SaveChanges();
        return Json(new { success = true });
    }
}