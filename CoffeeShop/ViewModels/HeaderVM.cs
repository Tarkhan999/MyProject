using FiorelloBackendPractice.ViewModels.Setting;

namespace FiorelloBackendPractice.ViewModels;

public class HeaderVM
{
    public SettingUIVM Setting { get; set; }
    public int BasketCount { get; set; }
    public double ProductPrice { get; set; }
}