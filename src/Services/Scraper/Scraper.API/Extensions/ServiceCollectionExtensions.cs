using Essentials;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scraper.API.Application.Queries;
using Scraper.Domain.AggregatesModel.ScraperAggregate;
using Scraper.Infrastructure;
using Scraper.Infrastructure.Repositories;
using System.Reflection;

namespace Scraper.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters();
                options.Authority = configuration.GetSection(LC.AUTH_TOKEN_SETTING_KEY)[LC.AUTHORITY_KEY];
                options.RequireHttpsMetadata = false;
                options.Audience = configuration.GetSection(LC.ALLOWED_ORIGIN_KEY)[LC.AUDIENCE_KEY];
            });

            return services;
        }

        public static IServiceCollection AddCacheDataStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(SC.REDIS_KEY);
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{section[SC.HOST_KEY]}:{section[SC.PORT_KEY]}";
            });
            services.AddScoped<IScrapeRequestCacheRepository, ScrapeRequestCacheRepository>();
            return services;
        }

        public static IServiceCollection AddDataStorage(this IServiceCollection services, IConfiguration configuration)
        {
            switch (configuration.GetSection(SC.DATA_STORAGE_KEY)[SC.TYPE_KEY])
            {
                case SC.NO_SQL_KEY:
                    AddNoSqlDataStorage(services, configuration);
                    break;
                case SC.RELATIONAL_KEY:
                    AddRelationalDataStorage(services, configuration);
                    break;
                default:
                    break;
            }
            return services;
        }

        private static void AddRelationalDataStorage(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCustonDbContext(configuration);
            services.AddSingleton<IScrapeRequestQueries>(x => { return new ScrapeRequestQueries(configuration.GetConnectionString(SC.DEFAULT_KEY)); });
            services.AddScoped<IScrapeRequestRepository, ScrapeRequestRepository>();
        }

        private static IServiceCollection AddCustonDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ScraperContext>(options =>
            {
                var connectionString = configuration.GetConnectionString(SC.DEFAULT_KEY);
                options.UseSqlServer(connectionString ?? string.Empty,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ScraperContext).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            },
            ServiceLifetime.Scoped
            );

            return services;
        }

        private static void AddNoSqlDataStorage(IServiceCollection services, IConfiguration configuration)
        {
            var GCSettings = configuration.GetSection(SC.FIREBASE_KEY);
            var dbBuilder = new FirestoreDbBuilder();
            dbBuilder.CredentialsPath = GCSettings[SC.CREDENTIAL_FILE_KEY];
            dbBuilder.ProjectId = GCSettings[SC.PROJECT_ID_KEY];

            FirestoreDb db = dbBuilder.Build();
            services.AddSingleton(db);
            services.AddSingleton<IScrapeRequestQueries>(x => { return new NoSqlScrapeRequestQueries(db); });
            services.AddScoped<IScrapeRequestRepository, NoSqlScrapeRequestRepository>();
        }
    }
}
