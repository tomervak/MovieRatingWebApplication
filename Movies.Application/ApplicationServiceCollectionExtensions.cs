using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Npgsql;

namespace Movies.Application;

public static class ApplicationServiceCollectionExtensions
{
    //addApplication service 

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_=>
            new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}