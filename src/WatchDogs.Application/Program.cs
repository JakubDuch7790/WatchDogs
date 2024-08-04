using Contracts;
using Infrastructure.DxTrade;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System.Net.Http.Headers;
using WatchDogs.Contracts;

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
    builder.Services.Configure<DxTradeConnectionOptions>(
        builder.Configuration.GetSection(nameof(DxTradeConnectionOptions)));

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

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
    builder.Services.AddTransient<DataGenerator>();

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    var app = builder.Build();

    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;

        var bogusDataGenerator = services.GetRequiredService<DataGenerator>();

        bogusDataGenerator.GenerateFakeTrade();

        var dxTradeAuthenticator = services.GetRequiredService<IDxTradeAuthenticator>();

        await dxTradeAuthenticator.AuthenticateAsync();

        var dxTradeClient = services.GetRequiredService<DxTradeClient>();

        await dxTradeClient.EstablishWebSocketConnectionAsync(dxTradeAuthenticator.AuthenticateAsync().Result);
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






