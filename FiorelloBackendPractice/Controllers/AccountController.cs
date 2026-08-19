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

    public AccountController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
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

    [HttpGet]
    public async Task<IActionResult> CreateRoles()
    {
        // 1. Rolleri oluştur
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

        // 2. Senin gerçek e-posta adresin:
        string myEmail = "zeynalabdiyevtrxan@gmail.com"; 

        var user = await _userManager.FindByEmailAsync(myEmail);

        if (user != null)
        {
            // Kullanıcı daha önce normal "Kayıt Ol" sayfasından kayıt olmuşsa, onu Admin yap.
            if (!await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
            {
                await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
        }
        else
        {
            // Kullanıcı veritabanında HİÇ YOKSA, sıfırdan oluştur.
            // DİKKAT: UserName'de boşluk olmamalı! ("TarkanZeynal" yapıldı)
            AppUser adminUser = new AppUser
            {
                UserName = "TarkanZeynal", 
                Email = "zeynalabdiyevtrxan@gmail.com",
                FullName = "Tarkan999",
            };

            var result = await _userManager.CreateAsync(adminUser, "Terxan993@");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
            }
        }

        return RedirectToAction("Index", "Home");
    }
    
    
}