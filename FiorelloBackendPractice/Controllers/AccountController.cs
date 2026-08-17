using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    public AccountController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
[HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task <IActionResult> Register(RegisterVM request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        AppUser user = new()
        {
            UserName = request.Email,
            Email = request.Email,
            FullName =  request.FullName

        };
      var result=  await _userManager.CreateAsync(user, request.Password);
      if (!result.Succeeded)
      {
          foreach (var item in result.Errors)
          {
              ModelState.AddModelError("",item.Description);
          }
          return View(request);
      }
        return RedirectToAction("Index", "Home");
    }
}