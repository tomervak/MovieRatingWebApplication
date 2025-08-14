namespace Movies.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";
    
    public static class Movies
    {
        private const string Base = $"{ApiBase}/movies";
    
        public const string GetMovie = $"{Base}/{{idOrSlug}}";
    
        public const string GetAllMovies = $"{Base}";
    
        public const string CreateMovie = $"{Base}";
    
        public const string UpdateMovie = $"{Base}/{{id}}";
    
        public const string DeleteMovie = $"{Base}/{{id:guid}}";
    
        public const string GetMovieRating = $"{Base}/{{id:guid}}/rating";
    
        public const string Rate = $"{Base}/{{id:guid}}/ratings";
    
        public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";
    }

    public static class Ratings
    {
        private const string Base = $"{ApiBase}/ratings";
    
        public const string GetUserRatings = $"{Base}/me";
    
    }
    
}