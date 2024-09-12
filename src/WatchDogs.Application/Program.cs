using Contracts;
using Infrastructure.DxTrade;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Net.Http.Headers;
using WatchDogs.Application;
using WatchDogs.Contracts;
using WatchDogs.Domain;
using WatchDogs.Infrastructure.FakeSource;
using WatchDogs.Persistence.EntityFramework;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


Log.Information("App is starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    //Database
    builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

    //Config options
    builder.Services.Configure<DxTradeConnectionOptions>(
        builder.Configuration.GetSection(nameof(DxTradeConnectionOptions)));

    builder.Services.Configure<FakeSourceOptions>(
        builder.Configuration.GetSection(nameof(FakeSourceOptions)));

    builder.Services.Configure<FakeTradegeneratorOptions>(
        builder.Configuration.GetSection(nameof(FakeTradegeneratorOptions)));

    //Other stuff
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    //DxTrade platform related stuff
    builder.Services.AddHttpClient(DxTradeConstants.DxTradeAuthenticationClient, client => {
        client.BaseAddress = new Uri("https://dxtrade.ftmo.com/api/auth/");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("User-Agent")));
    });
    builder.Services.AddSingleton<IDxTradeAuthenticator, DxTradeAuthenticator>();
    builder.Services.AddSingleton<ISessionTokenStorage, InMemorySessionTokenStorage>();
    builder.Services.AddSingleton<DxTradeClient>();

    //Services that query the Db
    //builder.Services.AddTransient<IDataInserter, DataInserter>();
    builder.Services.AddTransient<IDataLoader, DataLoader>();

    builder.Services.AddTransient<IFakeTradeGenerator, FakeTradeGenerator>();

    builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

    //Custom Services
    builder.Services.AddTransient<IWatcher, FakeSourceWatcher>();
    builder.Services.AddTransient<ISuspiciousDealDetector, SuspiciousDealDetector>();

    builder.Services.AddScoped<IUnitOfWork, EntityFrameworkUnitOfWork>();
    builder.Services.AddScoped<IDataInserter, DataInserter>();


    builder.Services.AddHostedService<WatcherBackgroundService>();

    var app = builder.Build();

    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;

        var SSD = services.GetRequiredService<ISuspiciousDealDetector>();

        var loadedTrades = await SSD.LoadDealsAsync();

    }
    

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Starting web application");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
