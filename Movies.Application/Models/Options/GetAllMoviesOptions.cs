namespace Movies.Application.Models.Options;

public class GetAllMoviesOptions
{
    public string? Title { get; set; }
    public int? YearOfRelease { get; set; }
    public Guid? UserId { get; set; }
}