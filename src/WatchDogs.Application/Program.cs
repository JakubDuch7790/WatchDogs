


using Contracts;
using Infrastructure.DxTrade;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DxTradeConnectionOptions>(
    builder.Configuration.GetSection(nameof(DxTradeConnectionOptions)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("DxTradeAuthenticatorClient", client => {
    client.BaseAddress = new Uri("https://dxtrade.ftmo.com/dxsca-web/");
});

//builder.Services.AddTransient<IDxTradeAuthenticator, DxTradeAuthenticator>();
builder.Services.AddSingleton<IDxTradeAuthenticator, DxTradeAuthenticator>();


//builder.Services.AddHttpClient<IDxTradeAuthenticator, DxTradeAuthenticator>(client =>
//    client.BaseAddress = new Uri("https://dxtrade.ftmo.com/dxsca-web/"));

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

app.Run();




