using eVotingSystemWebAPIsBackUp.Data;
using eVotingSystemWebAPIsBackUp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//  SERVICES 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  DB 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("EvotingConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("EvotingConnection"))
    )
);

builder.Services.AddDbContext<HomeAffairsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("HomeAffairsConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("HomeAffairsConnection"))
    )
);

//  JWT 
builder.Services.AddScoped<JwtService>();

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

//  FILE SERVICE 
builder.Services.AddScoped<FileStorageService>();

var app = builder.Build();

//  PIPELINE 

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader()
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();