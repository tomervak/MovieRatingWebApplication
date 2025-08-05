using System.Text;
using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory  _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into ratings(userid, movieid, rating)
                                                                         values(@userId, @movieId, @rating)
                                                                         on conflict (userid, movieid) do update 
                                                                            set rating = @rating
                                                                         """, new {userId, movieId, rating}));
        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<float?>("SELECT round(avg(r.rating),1) FROM ratings  r WHERE movieid = @movieId", new { movieId });
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select round(avg(rating),1),
                (select rating 
                from ratings
                where movieid = @movieId and userid = @userId limit 1)
            from ratings
            where movieid = @movieId

            """, new {movieId, userId}));
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(new StringBuilder().Append("""
                                                                              delete from ratings
                                                                              where movieid = @movieId
                                                                              and userid = @userId
                                                                              """)
            .ToString(), new {movieId, userId});
        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetUserRatingsAsync(Guid userId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
                                                                              select r.rating, r.movieid, m.slug
                                                                              from ratings r
                                                                              inner join movies m on r.movieid = m.id
                                                                              where userid = @userId
                                                                              """, new {userId}));
    }
}