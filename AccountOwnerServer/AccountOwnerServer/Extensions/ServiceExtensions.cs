﻿using Contracts.Log;
using Contracts.Repositories;
using Entities.Helpers;
using Entities.Models;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;

namespace AccountOwnerServer.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }

    public static void ConfigureIISIntegration(this IServiceCollection services)
    {
        services.Configure<IISOptions>(options =>
        {

        });
    }

    public static void ConfigureLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerManager, LoggerManager>();
    }

    public static void ConfigureMySqlContext(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration["MySqlConnection:ConnectionString"];

        services.AddDbContext<RepositoryContext>(opt => opt.UseMySql(connectionString,
            MySqlServerVersion.LatestSupportedServerVersion,
            opt => opt.MigrationsAssembly("Repository")));
    }

    public static void ConfigureRepositoryWrapper(this IServiceCollection services)
    {
        services.AddScoped<ISortHelper<Owner>, SortHelper<Owner>>();
        services.AddScoped<ISortHelper<Account>, SortHelper<Account>>();

        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }
}