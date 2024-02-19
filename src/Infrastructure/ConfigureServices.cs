using Business.Infrastructure;
using Infrastructure.Adapters;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddSingleton<NpgsqlDataSource>(_ => new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection")).Build());
        service.AddScoped<IDbConnectionAdapter, PostgresConnectionAdapter>();
        service.AddScoped<IMoviesRepository, MoviesRepository>();
        service.AddScoped<IUsersRepository, UsersRepository>();

        return service;
    }
}
