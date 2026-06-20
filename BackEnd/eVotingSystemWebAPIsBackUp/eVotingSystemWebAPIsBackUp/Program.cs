using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===================== SERVICES =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===================== DATABASES =====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("EvotingConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);

builder.Services.AddDbContext<HomeAffairsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("HomeAffairsConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);

// ===================== CORS =====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// ===================== SERVICES =====================
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<FaceRecognitionService>();
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddHttpClient();

// ===================== JWT =====================
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new Exception("JWT Key missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
        };
    });

// ===================== KESTREL =====================
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MinRequestBodyDataRate = null;
    options.Limits.MinResponseDataRate = null;
});

var app = builder.Build();

// ===================== PIPELINE =====================

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS (keep for production; safe on EC2 if SSL configured)
app.UseHttpsRedirection();

// ===================== STATIC FILES =====================
app.UseDefaultFiles();   // index.html support
app.UseStaticFiles();    // wwwroot

// uploads folder
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// ===================== ROUTING =====================
app.UseRouting();

// ===================== CORS =====================
app.UseCors("AllowAll");

// ===================== AUTH =====================
app.UseAuthentication();
app.UseAuthorization();

// ===================== ENDPOINTS =====================
app.MapControllers();

app.Run();