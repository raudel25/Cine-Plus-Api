using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Queries;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var appSettings = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var configuration = appSettings.Build();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDataBase(configuration);
builder.Services.AddAllServices();
builder.Services.AddMyAuthentication(configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme: Example \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

public static class ServiceCollectionExtensions
{
    public static void AddAllServices(this IServiceCollection services)
    {
        services.AddScoped<IMovieCommandHandler, MovieCommandHandler>();
        services.AddScoped<IMovieQueryHandler, MovieQueryHandler>();
        services.AddScoped<IMoviePropQueryHandler, MoviePropQueryHandler>();
        services.AddScoped<IMoviePropCommandHandler, MoviePropCommandHandler>();
        services.AddScoped<IShowMovieCommandHandler, ShowMovieCommandHandler>();
        services.AddScoped<IShowMovieQueryHandler, ShowMovieQueryHandler>();
        services.AddScoped<ICinemaQueryHandler, CinemaQueryHandler>();
        services.AddScoped<ICinemaCommandHandler, CinemaCommandHandler>();
        services.AddScoped<IDiscountQueryHandler, DiscountQueryHandler>();
        services.AddScoped<IDiscountCommandHandler, DiscountCommandHandler>();
        services.AddScoped<IAvailableSeatCommandHandler, AvailableSeatCommandHandler>();
        services.AddScoped<IAvailableSeatQueryHandler, AvailableSeatQueryHandler>();
        services.AddScoped<IAuthCommandHandler, AuthCommandHandler>();
        services.AddScoped<IAuthQueryHandler, AuthQueryHandler>();
        services.AddScoped<IPayOrderCommandHandler, PayOrderCommandHandler>();
        services.AddScoped<IPayOrderQueryHandler, PayOrderQueryHandler>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<SecurityService>();
        services.AddScoped<CacheService>();
        services.AddMemoryCache();
    }

    public static void AddDataBase(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 33));

        services.AddDbContext<CinePlusContext>(options =>
            options.UseMySql(connectionString, serverVersion));
    }

    public static void AddMyAuthentication(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    }
}