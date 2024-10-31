using Contracts;
using Infrastructure.DxTrade;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
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

    //DbContext specifically for storing SuspiciousTrades
    builder.Services.AddDbContext<SuspiciousTradesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

    //Config options
    builder.Services.Configure<DxTradeConnectionOptions>(
        builder.Configuration.GetSection(nameof(DxTradeConnectionOptions)));

    builder.Services.Configure<FakeSourceOptions>(
        builder.Configuration.GetSection(nameof(FakeSourceOptions)));

    builder.Services.Configure<FakeTradegeneratorOptions>(
        builder.Configuration.GetSection(nameof(FakeTradegeneratorOptions)));

    builder.Services.Configure<SuspiciousDealDetectorOptions>(
        builder.Configuration.GetSection(nameof(SuspiciousDealDetectorOptions)));

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
    //builder.Services.AddTransient<ITradeInserter, TradeInserter>(); resolved by factory
    builder.Services.AddTransient<ITradeLoader, TradeLoader>();

    builder.Services.AddTransient<IFakeTradeGenerator, FakeTradeGenerator>();

    //Logger

    builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddSingleton(Log.Logger);

    //Custom Services

    //Trades loading and inserting into Db
    builder.Services.AddScoped<ITradeInserter, TradeInserter>();
    builder.Services.AddScoped<IUnitOfWork, EntityFrameworkUnitOfWork>();
    builder.Services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

    //Suspicious trades inserting into Db
    builder.Services.AddScoped<ISuspiciousDealInserter, SuspiciousTradesInserter>();

    //Domain logic services
    builder.Services.AddTransient<IWatcher, FakeSourceWatcher>();
    builder.Services.AddTransient<ISuspiciousDealDetector, SuspiciousDealDetector>();

    builder.Services.AddHostedService<WatcherBackgroundService>();

    var app = builder.Build();

    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;

        var SDD = services.GetRequiredService<ISuspiciousDealDetector>();

        var loadedTrades = await SDD.LoadDealsAsync();

        var SS = await SDD.DetectSuspiciousDealsAsync(loadedTrades);

        await SDD.StoreSuspiciousTradesAsync(SS);
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
