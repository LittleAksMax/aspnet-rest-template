using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public string Slug => GenerateSlug();
    public float? Rating { get; set; }
    public int? UserRating { get; set; }
    public required int YearOfRelease { get; set; }
    public required List<string> Genres { get; init; } = new();

    /// <summary>
    /// Return slugged string for the Get endpoint.
    /// 'Nick The Greek' released in 2023 => nick-the-greek-2023
    /// This also removes special characters
    /// </summary>
    /// <returns>A slugged string.</returns>
    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower().Replace(' ', '-');
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9a-zA-Z-_ ]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}