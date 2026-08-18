using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM request)
    {
        if (!ModelState.IsValid) return View(request);

        AppUser user = new()
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
            return View(request);
        }

        // Kayıt olan kullanıcıyı otomatik login yapıp Home'a atar
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM request)
    {
        if (!ModelState.IsValid) return View(request);

        // 1. Önce Email, bulamazsa Username ile kullanıcıyı bul
        AppUser user = await _userManager.FindByEmailAsync(request.EmailOrUserName);
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(request.EmailOrUserName);
        }

        if (user == null)
        {
            ModelState.AddModelError("", "Email/Username or password incorrect");
            return View(request);
        }

        // 2. Şifre kontrolü ve Giriş
        var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Email/Username or password incorrect");
            return View(request);
        }

        // 3. Başarılı girişte doğrudan Home'a yönlendir
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}