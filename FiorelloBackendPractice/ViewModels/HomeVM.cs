using FiorelloBackendPractice.ViewModels.About;
using FiorelloBackendPractice.ViewModels.Blog;
using FiorelloBackendPractice.ViewModels.Category;

using FiorelloBackendPractice.Views.Product;

namespace FiorelloBackendPractice.ViewModels;

public class HomeVM
{
    public IEnumerable<BlogUIVM> Blogs { get; set; }
    public IEnumerable<CategoryUIVM> Categories { get; set; }
    public IEnumerable<ProductUIVM> Products { get; set; }
    public AboutUIVM AboutInfo { get; set; }
   
}