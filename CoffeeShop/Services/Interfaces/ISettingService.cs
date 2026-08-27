using FiorelloBackendPractice.ViewModels.Setting;

namespace FiorelloBackendPractice.Services.Interfaces;

public interface ISettingService
{
    Task<SettingUIVM> GetAsync();
}