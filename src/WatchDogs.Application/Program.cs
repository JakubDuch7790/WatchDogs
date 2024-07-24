


using Contracts;
using Infrastructure.DxTrade;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DxTradeConnectionOptions>(
    builder.Configuration.GetSection(nameof(DxTradeConnectionOptions)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient("DxTradeAuthenticationClient", client => {
    client.BaseAddress = new Uri("https://dxtrade.ftmo.com/api/auth/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("User-Agent")));
});

builder.Services.AddSingleton<IDxTradeAuthenticator, DxTradeAuthenticator>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var authenticator = services.GetRequiredService<IDxTradeAuthenticator>();

    await authenticator.AuthenticateAsync();
}

app.Run();




