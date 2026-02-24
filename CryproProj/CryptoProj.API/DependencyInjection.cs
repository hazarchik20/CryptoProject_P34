using CryptoProj.API.Validators;
using CryptoProj.Domain.Abstractions;
using CryptoProj.Domain.Models.Requests;
using CryptoProj.Domain.Services.Auth;
using CryptoProj.Domain.Services.Cryptocurrencies;
using CryptoProj.Domain.Services.Users;
using CryptoProj.Storage;
using CryptoProj.Storage.Repositories;
using CryptoProj.WebDataProvider.DataProviders;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CryptoProj.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddDbContext<CryptoContext>(opt => opt.UseInMemoryDatabase("local"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICryptocurrencyRepository, CryptocurrencyDataProvider>();
        services.AddScoped<ICryptoHistoryRepository, CryptoHistoryRepository>();
        services.AddScoped<ICryptoAnalyticsRepository, CryptoAnalyticsRepository>();

        services.AddScoped<UsersService>();
        services.AddScoped<CryptoAnalyticsService>();
        services.AddScoped<CryptocurrenciesService>();
        services.AddTransient<JwtTokenGenerator>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        return services;
    }
}