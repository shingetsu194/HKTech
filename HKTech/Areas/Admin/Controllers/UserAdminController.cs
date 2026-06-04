using HKTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UserAdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    public UserAdminController(UserManager<ApplicationUser> userManager)
        => _userManager = userManager;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        const int pageSize = 20;

        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(u =>
                u.FullName!.Contains(search) ||
                u.Email!.Contains(search));

        var total = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Load roles cho từng user
        var userRoles = new Dictionary<string, IList<string>>();
        foreach (var u in users)
            userRoles[u.Id] = await _userManager.GetRolesAsync(u);

        ViewBag.UserRoles  = userRoles;
        ViewBag.Search     = search;
        ViewBag.Page       = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.Total      = total;

        return View(users);
    }
}
