namespace FiorelloBackendPractice.Models;

public class About:BaseEntity
{
    public string Title { get; set; } 
    public string HighlightedText { get; set; } 
    public string Description { get; set; } 
    public string PointText { get; set; } 
    public string ImageUrl { get; set; }
}