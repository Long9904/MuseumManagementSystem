using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MuseumSystem.Api;
using MuseumSystem.Api.Middleware;
using MuseumSystem.Application.Validation;
using MuseumSystem.Infrastructure.DatabaseSetting;

var builder = WebApplication.CreateBuilder(args);

//Controllers + Validation
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "MuseumSystem API", Version = "v1" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

//Authentication: Google login , JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var googleSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleSection["ClientId"];
    options.ClientSecret = googleSection["ClientSecret"];
    options.CallbackPath = "/auth/google/callback";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "")
        ),
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

//EF Core + DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddConfig(builder.Configuration);
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

//Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
