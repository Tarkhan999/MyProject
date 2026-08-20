using FiorelloBackendPractice.Helpers.Enums;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Account;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;

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

        await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = Url.Action(nameof(ConfirmEmail), "Account", new
        {
            userId = user.Id,
            token,
        }, Request.Scheme, Request.Host.ToString());

        // Email Mesajı Oluşturma
        string senderEmail = _config["SmtpSettings:SenderEmail"];
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(senderEmail));
        email.To.Add(MailboxAddress.Parse(user.Email));
        email.Subject = "Email Confirmation";
        email.Body = new TextPart(TextFormat.Html) { Text = $"<a href='{link}'>Click Here</a>" };

        // Email Gönderme
        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

        string smtpServer = _config["SmtpSettings:Server"];
        int smtpPort = int.Parse(_config["SmtpSettings:Port"] ?? "587");
        string smtpPassword = _config["SmtpSettings:Password"];

        await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, smtpPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        return RedirectToAction(nameof(VerifyEmail));
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            return BadRequest();
        }

        AppUser user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        await _signInManager.SignInAsync(user, false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult VerifyEmail()
    {
        return View();
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
                EmailConfirmed = true // E-posta onayını aktif eder
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
}