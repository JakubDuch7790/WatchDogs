


using Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DxTradeConnectionOptions>(
    builder.Configuration.GetSection(nameof(DxTradeConnectionOptions)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient("myClient", client => {
    client.BaseAddress = new Uri("https://dxtrade.ftmo.com/dxsca-web/");
});

var app = builder.Build();

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
