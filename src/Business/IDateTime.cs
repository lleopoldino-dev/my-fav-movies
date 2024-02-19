namespace Business;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
