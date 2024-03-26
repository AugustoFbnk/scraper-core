using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scraper.BackgroundTasks;
using Scraper.BackgroundTasks.Extensions;
using Scraper.BackgroundTasks.Options;
using Scraper.BackgroundTasks.Services.BackgroundServices;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((host, builder) =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddEnvironmentVariables();
        builder.AddCommandLine(args);
        builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"));
    })
    .ConfigureServices((host, services) =>
    {
        services
       .AddHostedService<ScraperBackgroundService>()
       .AddServices(host.Configuration)
       .AddOptions(host.Configuration)
       .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))
       .Configure<HostOptions>(hostOptions =>
       {
           //not stop host when an unhandled exception occurs in a Background service
           hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
       });
    })
    .ConfigureLogging(logging =>
        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
               .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Error)
               .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Critical))
    .Build();

new PostHostBuildConfig(host.Services.GetRequiredService<IOptions<FirebaseOptions>>()).Setup();

await host.RunAsync();
