using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Projendeki gerçek namespace'lere göre:
using IdentityService.Infrastructure.Data;            // AppDbContext
using IdentityService.Infrastructure.Repositories;    // UserRepository
using IdentityService.Infrastructure.Auth;            // TokenService
using IdentityService.Application.Interfaces;         // IUserRepository, ITokenService
using IdentityService.Application.Services;           // AuthService

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
));

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (SQLite)
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? "Data Source=identity.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connStr));

// DI kayıtları
builder.Services.AddScoped<IUserRepository, UserRepository>();

// TokenService kurucusu parametre istiyor → factory ile kaydediyoruz
builder.Services.AddScoped<ITokenService>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var issuer = cfg["Jwt:Issuer"] ?? "identity";
    var audience = cfg["Jwt:Audience"] ?? "clients";
    var secret = cfg["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured");
    return new TokenService(issuer, audience, secret);
});

builder.Services.AddScoped<AuthService>();

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
var jwtIssuer   = jwt["Issuer"]   ?? "identity";
var jwtAudience = jwt["Audience"] ?? "clients";
var jwtSecret   = jwt["Secret"]   ?? throw new InvalidOperationException("Jwt:Secret not configured");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false; // dev
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // dev

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();