using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MuseumSystem.Api;
using MuseumSystem.Api.Middleware;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Utils;
using MuseumSystem.Application.Validation;
using MuseumSystem.Domain.Enums.EnumConfig;
using MuseumSystem.Domain.Options;
using MuseumSystem.Infrastructure.DatabaseSetting;


var builder = WebApplication.CreateBuilder(args);

//Controllers + Validation
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
    .AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.Converters.Add(
            new ExclusiveEnumConverterFactory(
                excludeFromString: new[] { typeof(StatusCodeHelper) }
            ));
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

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();

            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                IsSuccess = false,
                Errors = "Unauthorized or missing token",
                StatusCode = context.Response.StatusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        OnAuthenticationFailed = async context =>
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var message = context.Exception?.GetType().Name switch
            {
                "SecurityTokenExpiredException" => "Token expired",
                "SecurityTokenInvalidSignatureException" => "Invalid token signature",
                _ => "Invalid token"
            };

            var response = new
            {
                IsSuccess = false,
                Errors = message,
                StatusCode = context.Response.StatusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        OnForbidden = async context =>
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                IsSuccess = false,
                Errors = "You do not have permission to access this resource",
                StatusCode = context.Response.StatusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    };
});

//Add Dependency Injection
builder.Services.AddConfig(builder.Configuration);

//Config AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// EF Core SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis Cache
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Redis"));

builder.Services.AddConfig(builder.Configuration);
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

//Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<MuseumContextMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
