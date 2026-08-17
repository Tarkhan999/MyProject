using Microsoft.AspNetCore.Identity;

namespace FiorelloBackendPractice.Models;

public class AppUser: IdentityUser
{
    public string FullName { get; set; }
}