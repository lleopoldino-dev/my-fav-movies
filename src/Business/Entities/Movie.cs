namespace Business.Models;

public class Movie : BaseEntity
{
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Category { get; set; }
}