namespace Movies.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Movies
    {
        private const string Base = $"{ApiBase}/movies";
        
        public const string GetMovie = $"{Base}/{{id:guid}}";
        
        public const string GetAllMovies = $"{Base}";
        
        public const string CreateMovie = $"{Base}";
        
        public const string UpdateMovie = $"{Base}/{{id}}";
        
        public const string DeleteMovie = $"{Base}/{{id:guid}}";
    }
}