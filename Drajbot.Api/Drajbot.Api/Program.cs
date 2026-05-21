using Drajbot.Api.Data;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi; // <-- OVO JE JEDINI NAMESPACE SADA, NEMA .Models!
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1. REGISTRACIJA SERVISA
// =======================================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IAuthService, Drajbot.Api.Services.Auth.AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- NOVI SWAGGER ZA v10.x i OpenApi 3.x ---
builder.Services.AddSwaggerGen(c =>
{
    // 1. Definicija Katanca
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Unesite vaš JWT token ovde (Swagger sam dodaje reč Bearer).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // 2. Primena Katanca (NOVA SINTAKSA ZA v10)
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>() // <--- ISPRAVLJENO: Sada je čista Lista umesto Niza!
        }
    });
});
// ---------------------------------------------------

// JWT Autentifikacija
var jwtSecret = builder.Configuration["Jwt:Secret"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
    });

// =======================================================
// 2. ZAKLJUČAVANJE APLIKACIJE
// =======================================================
var app = builder.Build();

// =======================================================
// 3. HTTP PIPELINE
// =======================================================
app.UseMiddleware<Drajbot.Api.Middlewares.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Provera tokena i uloga
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();