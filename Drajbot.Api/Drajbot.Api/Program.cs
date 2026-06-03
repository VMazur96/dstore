using Drajbot.Api.Data;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
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
builder.Services.AddScoped<IOrderService, Drajbot.Api.Services.Orders.OrderService>();
builder.Services.AddScoped<IUploadService, Drajbot.Api.Services.Uploads.UploadService>();
builder.Services.AddScoped<IReviewService, Drajbot.Api.Services.Reviews.ReviewService>();
builder.Services.AddScoped<IEmailService, Drajbot.Api.Services.Emails.EmailService>();
builder.Services.AddScoped<IWishlistService, Drajbot.Api.Services.Wishlist.WishlistService>();

builder.Services.AddHttpClient(); // <-- OVO PALI INTERNET KONEKCIJU ZA NAŠ SERVER

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Definisanje CORS polise za Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // Ovde idu adrese tvog React sajta (dodali smo standardne portove)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // <--- OVO JE KLJUČNO! Dozvoljava HttpOnly Cookies!
    });
});

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

        // DODATO: Kažemo serveru da automatski izvuče token iz Cookie-ja
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["jwt"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("login_policy", opt =>
    {
        opt.TokenLimit = 5;          // Korisnik ima 5 pokušaja "u džepu"
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;          // Ne čekaj u redu, odmah odbij ako nema tokena
        opt.ReplenishmentPeriod = TimeSpan.FromMinutes(5); // Krediti se pune na 5 minuta
        opt.TokensPerPeriod = 5;     // Dopuni svih 5 kredita na 5 minuta
        opt.AutoReplenishment = true;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429; // Too Many Requests
        await context.HttpContext.Response.WriteAsJsonAsync(new { poruka = "Previše pokušaja. Sačekajte 5 minuta." });
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

app.UseStaticFiles();

app.UseCors("FrontendCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();