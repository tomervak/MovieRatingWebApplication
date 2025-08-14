using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Movie?> GetMovieByIdAsync(Guid id, Guid? userId = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>("""
                                                                      SELECT m.*  , round(avg(r.rating),1) as rating, myr.rating as userrating
                                                                      FROM movies m
                                                                      left join ratings r on m.id = r.movieid
                                                                      left join ratings myr on m.id = myr.movieid 
                                                                                                  and myr.userid = @userId 
                                                                      WHERE id = @Id
                                                                      group by id,userrating
                                                                      """, new { id, userId });

        if (movie == null)
            return null;

        var genres = await connection.QueryAsync<string>("SELECT name FROM genres where movieId = @Id", new {  Id = id });
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync(GetAllMoviesOptions options)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var orderClause = string.Empty;
        if (options.SortField is not null)
        {
            orderClause = $"""
                           ,m.{options.SortField}
                           order by m.{options.SortField} {(options.SortOrder== SortOrder.Ascending ? "asc" : "desc")} 
                           """;
        }
        
        var result = await connection.QueryAsync(new CommandDefinition($"""
                                                                       select m.*,
                                                                              string_agg(distinct g.name, ',') as genres,
                                                                              round(avg(r.rating),1) as rating,
                                                                              myr.rating as userrating
                                                                       from movies m
                                                                       left join genres g on m.id = g.movieId
                                                                       left join ratings r on m.id = r.movieid
                                                                       left join ratings myr on m.id = myr.movieid and myr.userid = @userId
                                                                       where (@title is null or m.title like ('%' || @title || '%'))
                                                                       and (@yearofrelease is null or m.yearofrelease = @yearofrelease)
                                                                       and (@director is null or m.director = @director)
                                                                       group by id,userrating {orderClause}
                                                                       limit @pageSize
                                                                       offset @pageOffSet
                                                                       """, new
        {
            userId= options.UserId, title = options.Title,
            yearofrelease = options.YearOfRelease, director = options.Director,
            pageSize=options.PageSize,
            pageOffSet= (options.Page -1) * options.PageSize
        }));
        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            Director = x.director,
            YearOfRelease = x.yearofrelease,
            Rating = (float?)x.rating,
            UserRating = (int?)x.userrating,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> CreateMovieAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into movies (id,title,director,slug,yearofrelease)
                                                                         values (@Id,@Title,@Director,@Slug,@YearOfRelease)
                                                                         """, movie));
        if (result > 0)
        {
            foreach (var gerne in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                    insert into genres (movieId,name)
                                                                    values (@MovieId,@Name)
                                                                    """, new { MovieId = movie.Id, Name = gerne }));
            }
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> UpdateMovieAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genres where movieId = @id
                                                            """, new { id = movie.Id }));
        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genres (movieId,name)
                                                                values (@MovieId,@Name)
                                                                """, new { MovieId = movie.Id, Name = genre }));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                            update movies set title= @Title,director= @Director, slug = @Slug, yearofrelease= @YearOfRelease
                                                                            where id = @Id
                                                                         """,movie));
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteMovieAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genres where movieId = @Id
                                                            """, new { Id = id }));
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from movies where id = @Id
                                                                         """, new { id }));
        transaction.Commit();
        return result > 0;
    }

    public async Task<Movie?> GetMovieBySlugAsync(string slug,  Guid? userId = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>("""
                                                                      SELECT m.*  , round(avg(r.rating),1) as rating, myr.rating as userrating
                                                                      FROM movies m
                                                                      left join ratings r on m.id = r.movieid
                                                                      left join ratings myr on m.id = myr.movieid 
                                                                                                  and myr.userid = @userId 
                                                                      WHERE slug = @slug
                                                                      group by id,userrating
                                                                      """, new { slug, userId });

        if (movie == null)
            return null;

        var genres =
            await connection.QueryAsync<string>("SELECT name FROM genres where movieId = @Id", new { Id = movie.Id });
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }
        return movie;
    }

    public async Task<bool> ExistByMovieIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from movies WHERE id = @id
                                                                               """, new { id }));
    }

    public async Task<int> GetCountAsync(string? title, int? yearOfRelease)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleAsync<int>(new CommandDefinition("""
                                                                            select count(id) from movies
                                                                            where (@title is null or title like ('%' || @title || '%'))
                                                                            and (@yearOfRelease is null or yearOfRelease = @yearOfRelease)
                                                                            """, new {title, yearOfRelease}));

    }
}