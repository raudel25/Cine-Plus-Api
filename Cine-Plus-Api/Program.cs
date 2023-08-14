using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Services;


var builder = WebApplication.CreateBuilder(args);

var appSettings = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");


// Add services to the container.

builder.Services.AddControllers();

var configuration = appSettings.Build();
var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CinePlusContext>(options =>
    options.UseMySQL(connectionString!));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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