using FiorelloBackendPractice.Helpers.Enums;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;

    public AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _config = config;
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

        // Kullanıcıya rol ataması
        await _userManager.AddToRoleAsync(user, Roles.Member.ToString());

        // Kayıt başarılı olduğunda kullanıcıyı doğrudan sisteme giriş yaptır
        await _signInManager.SignInAsync(user, isPersistent: false);
        
        // Ana sayfaya yönlendir
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

        AppUser user = await _userManager.FindByEmailAsync(request.EmailOrUserName)
            ?? await _userManager.FindByNameAsync(request.EmailOrUserName);

        if (user == null)
        {
            ModelState.AddModelError("", "Email/Username or password incorrect");
            return View(request);
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Email/Username or password incorrect");
            return View(request);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> CreateRoles()
    {
        foreach (var item in Enum.GetValues(typeof(Roles)))
        {
            if (!await _roleManager.RoleExistsAsync(item.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = item.ToString()
                });
            }
        }

        string myEmail = _config["AdminSettings:Email"];
        var user = await _userManager.FindByEmailAsync(myEmail);

        if (user != null)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            if (!await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
            {
                await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
        }
        else
        {
            AppUser adminUser = new AppUser
            {
                UserName = _config["AdminSettings:UserName"],
                Email = myEmail,
                FullName = _config["AdminSettings:FullName"],
                EmailConfirmed = true 
            };

            string adminPassword = _config["AdminSettings:Password"];
            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
            }
        }

        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    public async Task<IActionResult> QuickSubscribeLogin(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Json(new { success = false, message = "Email cannot be empty." });
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Json(new { success = true, isLogin = true, message = "Welcome back! Logging you in..." });
        }

        return Json(new { success = true, isLogin = false, message = "Thank you for subscribing to our club!" });
    }
}