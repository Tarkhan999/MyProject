using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Subscribe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;
[Area("Admin")]
public class SubscriberController : Controller
{
    private readonly AppDbContext _dbContext;
    public SubscriberController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task <IActionResult> Index()
    {
        List<SubscriberVM> subscribers = await _dbContext.Subscribers.OrderByDescending(s => s.CreatedAt).Select(s =>
            new SubscriberVM
            {
                Id = s.Id,
                Email = s.Email,
                CreatedAt = s.CreatedAt,
            }).ToListAsync();
        return View(subscribers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        Subscriber? subscriber = await _dbContext.Subscribers.FindAsync(id);
        if (subscriber == null) return NotFound();
        _dbContext.Subscribers.Remove(subscriber);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}