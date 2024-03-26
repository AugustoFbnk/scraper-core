using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Scraper.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Scraper.Domain.AggregatesModel.ScraperAggregate;
using Scraper.Infrastructure.Repositories;
using Scraper.Domain.Abstractions.DomainServices;
using Scraper.Abstractions.Domain.DomainServices.Services;
using Scraper.Domain.DomainServices;
using Scraper.BackgroundTasks.Services.Pooling;
using Scraper.BackgroundTasks.Services.Browsering;
using Scraper.BackgroundTasks.Abstractions.Services.Pooling;
using Scraper.BackgroundTasks.Abstractions.Services.Browsering;
using Scraper.BackgroundTasks.Options;
using Google.Cloud.Firestore;
using Essentials;

namespace Scraper.BackgroundTasks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IScraperService, ScraperService>()
                .AddScoped<IFcmService, FcmService>()
                .AddSingleton<IBrowserLoader, BrowserLoader>()
                .AddSingleton<IBrowserPool, BrowserPool>()
                .AddScoped<IScrapeCacheManagementService, ScrapeCacheManagementService>()
                .AddDataStorage(configuration)
                .AddCacheDataStorage(configuration);
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
            services.AddScoped<IScrapeRequestRepository, ScrapeRequestRepository>();
        }

        private static void AddNoSqlDataStorage(IServiceCollection services, IConfiguration configuration)
        {
            var GCSettings = configuration.GetSection(SC.FIREBASE_KEY);
            var dbBuilder = new FirestoreDbBuilder();
            dbBuilder.CredentialsPath = GCSettings[SC.CREDENTIAL_FILE_KEY];
            dbBuilder.ProjectId = GCSettings[SC.PROJECT_ID_KEY];

            FirestoreDb db = dbBuilder.Build();
            services.AddSingleton(db);
            services.AddScoped<IScrapeRequestRepository, NoSqlScrapeRequestRepository>();
        }

        private static IServiceCollection AddCustonDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ScraperContext>(options =>
            {
                var connectionString = configuration.GetConnectionString(SC.DEFAULT_KEY);

                options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ScraperContext).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            },
            ServiceLifetime.Transient
            );

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<PuppeteerSharpOptions>().Bind(configuration.GetSection(PuppeteerSharpOptions.OPTION_NAME));
            services.AddOptions<FirebaseOptions>().Bind(configuration.GetSection(FirebaseOptions.OPTION_NAME));
            return services;
        }
    }
}
