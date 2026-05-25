using Drajbot.Api.Data;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1. REGISTRACIJA SERVISA (Dependency Injection Kontejner)
// =======================================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// OVDE REGISTRUJEMO NAŠE SERVISE:
builder.Services.AddScoped<IAuthService, Drajbot.Api.Services.Auth.AuthService>();
builder.Services.AddScoped<ICatalogService, Drajbot.Api.Services.Catalogs.CatalogService>(); // <-- NOVI SERVIS!
builder.Services.AddScoped<IFortniteShopService, Drajbot.Api.Services.Fortnite.FortniteShopService>();

builder.Services.AddHttpClient(); // <-- OVO PALI INTERNET KONEKCIJU ZA NAŠ SERVER

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- SWAGGER ---
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Unesite vaš JWT token ovde (Swagger sam dodaje reč Bearer).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
// ---------------------------------------------------

// --- JWT Autentifikacija ---
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();