using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerMinder.Core;
using PowerMinder.Core.Entity;
using PowerMinder.Engine.Helpers;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();
        services.Configure<PowerMinderSettings>(configurationRoot.GetSection("AppSettings"));

        IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        string connectionString = configuration.GetConnectionString("PowerMinder_ConnectionString");
        string connectionStringAcc = configuration.GetConnectionString("Account_ConnectionString");

        services.AddDbContext<EntityContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);
        services.AddDbContext<AccountContext>(options => options.UseSqlServer(connectionStringAcc), ServiceLifetime.Singleton);

        services.AddScoped<IUtility, Utility>();

        services.AddScoped<ReminderCore, ReminderCore>();


    })
    .Build();

host.Run();
