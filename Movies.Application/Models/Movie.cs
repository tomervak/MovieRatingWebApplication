using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public class Movie
{
    public required Guid Id {get;init;}
    
    public required string Title {get;set;}
    
    public required string Director {get;set;}

    public string Slug => GenerateSlug();

    public required int YearOfRelease{get;set;}

    public required List<string> Genres { get; init; } = new();
    
    private string GenerateSlug()
    {
        var sluggedTitle = Regex.Replace(Title, @"[^a-z0-9\s-]", "").ToLower().Replace(" ","-");
        var sluggedDirector = Regex.Replace(Director, @"[^a-z0-9\s-]", "").ToLower().Replace(" ","-");
        return $"{sluggedTitle}-{sluggedDirector}-{YearOfRelease}";
    }
}