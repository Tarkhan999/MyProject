namespace FiorelloBackendPractice.ViewModels.Blog;

public class BlogVM
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Image { get; set; } = null!;
    public string Link { get; set; } = null!;
    public DateTime DateCreated { get; set; }
    public int ViewsCount { get; set; }
}