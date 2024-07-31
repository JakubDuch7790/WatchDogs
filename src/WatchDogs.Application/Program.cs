using Contracts;
using Infrastructure.DxTrade;
using Serilog;
using Serilog.Events;
using System.Net.Http.Headers;

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    //    var logger = new LoggerConfiguration()
    //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    //.Enrich.FromLogContext()
    //.WriteTo.Console()
    //.CreateBootstrapLogger();




    //builder.Host.UseSerilog((context, services, configuration) => configuration
    //.ReadFrom.Configuration(context.Configuration)
    //.ReadFrom.Services(services)
    //.Enrich.FromLogContext()
    //.WriteTo.Console());

    ////--> Serilog Configuration below
    //var logger = new LoggerConfiguration()
    //.ReadFrom.Configuration(builder.Configuration)
    //.Enrich.FromLogContext()
    //.CreateLogger();

    //builder.Logging.ClearProviders();
    //builder.Logging.AddSerilog(logger);


    //builder.Host.UseSerilog();

    builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

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

    builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

    var app = builder.Build();

    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;

        var dxTradeAuthenticator = services.GetRequiredService<IDxTradeAuthenticator>();

        await dxTradeAuthenticator.AuthenticateAsync();
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






