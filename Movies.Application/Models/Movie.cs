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
        var sluggedTitle = Regex.Replace(Title, @"[^0-9A-Za-z _-]", "").ToLower().Replace(" ","-");
        var sluggedDirector = Regex.Replace(Director, @"^0-9A-Za-z _-]", "").ToLower().Replace(" ","-");
        return $"{sluggedTitle}-{sluggedDirector}-{YearOfRelease}";
    }
}