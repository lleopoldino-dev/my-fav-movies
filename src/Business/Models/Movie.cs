namespace Business.Models;

public class Movie : BaseModel
{
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Category { get; set; }
}