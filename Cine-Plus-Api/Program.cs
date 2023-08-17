using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Queries;

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

builder.Services.AddScoped<IMovieCommandHandler, MovieCommandHandler>();
builder.Services.AddScoped<IMovieQueryHandler, MovieQueryHandler>();
builder.Services.AddScoped<IMoviePropQueryHandler, MoviePropQueryHandler>();
builder.Services.AddScoped<IMoviePropCommandHandler, MoviePropCommandHandler>();
builder.Services.AddScoped<IShowMovieCommandHandler, ShowMovieCommandHandler>();
builder.Services.AddScoped<IShowMovieQueryHandler, ShowMovieQueryHandler>();
builder.Services.AddScoped<ICinemaQueryHandler, CinemaQueryHandler>();
builder.Services.AddScoped<ICinemaCommandHandler, CinemaCommandHandler>();
builder.Services.AddScoped<IDiscountQueryHandler, DiscountQueryHandler>();
builder.Services.AddScoped<IDiscountCommandHandler, DiscountCommandHandler>();
builder.Services.AddScoped<IAvailableSeatsCommandHandler, AvailableSeatsCommandHandler>();
builder.Services.AddScoped<IAvailableSeatQueryHandler, AvailableSeatQueryHandler>();
builder.Services.AddScoped<IAuthCommandHandler, AuthCommandHandler>();
builder.Services.AddScoped<IAuthQueryHandler, AuthQueryHandler>();
builder.Services.AddScoped<SecurityService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
        };
    });


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();