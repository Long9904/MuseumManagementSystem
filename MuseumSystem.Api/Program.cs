using Microsoft.EntityFrameworkCore;
using MuseumSystem.Infrastructure.DatabaseSetting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var mysqlVersion = builder.Configuration["DatabaseSettings:MySqlVersion"];

// Check null connection string & version

if (string.IsNullOrEmpty(mysqlVersion) || !Version.TryParse(mysqlVersion, out _))
{
    throw new InvalidOperationException("MySQL version is not specified or invalid in configuration. Please check appsetting or appsetting dev");
}


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(mysqlVersion))));

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
