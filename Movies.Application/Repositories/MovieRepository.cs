using System.Transactions;
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

    public async Task<Movie?> GetMovieByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>("SELECT * FROM movies WHERE Id = @Id",
            new { Id = id });

        if (movie == null)
            return null;

        var genres = await connection.QueryAsync<string>("SELECT name FROM genres where movieId = @Id", new {  Id = id });
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync(new CommandDefinition("""
                                                                       select m.*, string_agg(g.name, ',') as genres
                                                                       from movies m
                                                                       left join genres g on m.id = g.movieId
                                                                       group by id
                                                                       """));
        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            Director = x.director,
            YearOfRelease = x.yearofrelease,
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

    public async Task<Movie?> GetMovieBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>("SELECT * FROM movies WHERE Slug = @Slug",
            new { slug });

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
}