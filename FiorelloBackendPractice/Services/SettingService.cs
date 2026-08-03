using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Setting;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Services;

public class SettingService : ISettingService
{
    private readonly AppDbContext _dbContext;

    public SettingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SettingUIVM> GetAsync()
    {
        var setting = await _dbContext.Settings.Select(s => new SettingUIVM
        {
            HeaderLogo = s.HeaderLogo,
            Phone = s.Phone,
            Email = s.Email
        }).FirstOrDefaultAsync();
        return setting;
    }
}