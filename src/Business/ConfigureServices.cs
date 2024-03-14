using Business.Services;
using Business.Services.MovieServices;
using Business.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;

namespace Business;

public static class ConfigureServices
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddTransient<IDateTime, DateTimeService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}
