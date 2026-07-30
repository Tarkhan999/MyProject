using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Services;

public class SliderService:ISliderService
{
    private readonly AppDbContext _dbContext;

    public SliderService(AppDbContext context)
    {
        _dbContext = context;
    }
    public async Task<IEnumerable<Slider>> GetAllAsync()
    {
        return await  _dbContext.Sliders.ToListAsync();
    }

    public async Task<SliderInfo> GetInfoAsync()
    {
        return await _dbContext.SliderInfos.FirstOrDefaultAsync();
    }
}