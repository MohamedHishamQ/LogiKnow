using AspNetCoreRateLimit;
using LogiKnow.API.Middleware;
using LogiKnow.Application;
using LogiKnow.Domain.Interfaces;
using LogiKnow.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== Serilog =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/logiknow-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// ===== Application + Infrastructure DI =====
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ===== JWT Authentication =====
var jwtKey = builder.Configuration["JwtSettings:SecretKey"] ?? "DEFAULT_SECRET_KEY_REPLACE_ME_256BIT_MINIMUM!!";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "logiknow-api",
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "logiknow-client",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ===== Rate Limiting =====
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new() { Endpoint = "GET:/api/search*", Period = "1s", Limit = 10 },
        new() { Endpoint = "GET:/api/terms/*/explain", Period = "1s", Limit = 5 }
    };
});
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

// ===== CORS =====
var allowedOrigins = builder.Configuration["AllowedCorsOrigins"]?.Split(',') ??
    new[] { "http://localhost:3000" };
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ===== Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LogiKnow API",
        Version = "v1",
        Description = "Logistics Knowledge Platform API"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ===== Middleware Pipeline =====
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogiKnow API v1"));
}

app.UseHttpsRedirection();
app.UseCors();
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ===== Seed Roles & Ensure ES Indices =====
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
    string[] roles = { "Admin", "Moderator", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role));
    }

    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<LogiKnow.Infrastructure.Persistence.AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to initialize the in-memory database on startup.");
    }

    try
    {
        var searchService = scope.ServiceProvider.GetRequiredService<ISearchService>();
        await searchService.EnsureIndicesCreatedAsync();
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to create Elasticsearch indices on startup. Search features may be unavailable.");
    }
}

Log.Information("LogiKnow API starting on {Urls}", string.Join(", ", app.Urls));
app.Run();
