using HKTech.Models;
using HKTech.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HKTech.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser>  _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser>  userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager  = userManager;
        _signInManager = signInManager;
    }

    // ── Login Select ───────────────────────────────────────────────────
    [HttpGet]
    public IActionResult LoginSelect()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View();
    }

    // ── Login (Customer) ─────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(vm);

        var result = await _signInManager.PasswordSignInAsync(
            vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            TempData["Success"] = "★ Chào mừng trở lại!";
            return LocalRedirect(returnUrl ?? "/");
        }

        if (result.IsLockedOut)
            ModelState.AddModelError("", "Tài khoản tạm khóa do đăng nhập sai nhiều lần.");
        else
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");

        return View(vm);
    }

    // ── Admin Login ─────────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult AdminLogin(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminLogin(LoginViewModel vm, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(vm);

        var result = await _signInManager.PasswordSignInAsync(
            vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // Kiểm tra đúng role Admin
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Success"] = "💀 Xin chào Admin! STAY DETERMINED.";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            // Không phải Admin → đăng xuất và báo lỗi
            await _signInManager.SignOutAsync();
            ModelState.AddModelError("", "Tài khoản này không có quyền Admin.");
            return View(vm);
        }

        if (result.IsLockedOut)
            ModelState.AddModelError("", "Tài khoản tạm khóa do đăng nhập sai nhiều lần.");
        else
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");

        return View(vm);
    }

    // ── Register ──────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = new ApplicationUser
        {
            UserName = vm.Email,
            Email    = vm.Email,
            FullName = vm.FullName,
        };

        var result = await _userManager.CreateAsync(user, vm.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Success"] = "★ Đăng ký thành công! Chào mừng bạn đến với HKTech!";
            return RedirectToAction("Index", "Home");
        }

        foreach (var err in result.Errors)
            ModelState.AddModelError("", err.Description);

        return View(vm);
    }

    // ── Logout ────────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    // ── Profile ───────────────────────────────────────────────────────────
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    // ── Access Denied ─────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult AccessDenied() => View();
}
