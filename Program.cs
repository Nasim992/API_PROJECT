using Dextor.API.Models.BindingModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Dextor.API.Data.IRepositories;
using Dextor.API.Data.Repositories;

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddEndpointsApiExplorer();

// ==========================================================
// 1. ADD YOUR DATABASE CONTEXTS HERE (WITH TIMEOUTS)
// ==========================================================

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WEBConnection"),
        sqlOptions => sqlOptions.CommandTimeout(60000)));

//builder.Services.AddDbContext<POSDBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("POSConnection"),
//        sqlOptions => sqlOptions.CommandTimeout(60000)));

//builder.Services.AddDbContext<DWDBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DWDBConnection"),
//        sqlOptions => sqlOptions.CommandTimeout(60000)));

//builder.Services.AddDbContext<OracleDBContext>(options =>
//    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));
// ==========================================================


// ==========================================================
// 2. REGISTER YOUR REPOSITORIES HERE (If you use them)
// ==========================================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRegistrationRepository, UserRegistrationRepository>();
builder.Services.AddScoped<IReportLogRepository, ReportLogRepository>();
// ==========================================================


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dextor API", Version = "v1" });

    // 2. UPDATED SWAGGER CONFIG (Foolproof HTTP Scheme)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, // Changed from ApiKey to Http
        Scheme = "bearer"               // Swagger will auto-add "Bearer " for you
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero // Add this to force exact expiration
    };

    // 3. ADDED DIAGNOSTIC EVENTS TO CATCH THE ERROR
    //options.Events = new JwtBearerEvents
    //{
    //    OnAuthenticationFailed = context =>
    //    {
    //        Console.WriteLine("\n=== TOKEN VALIDATION FAILED ===");
    //        Console.WriteLine(context.Exception.Message);
    //        Console.WriteLine("===============================\n");
    //        return Task.CompletedTask;
    //    },
    //    OnTokenValidated = context =>
    //    {
    //        Console.WriteLine("\n=== TOKEN VALIDATED SUCCESSFULLY ===\n");
    //        return Task.CompletedTask;
    //    }
    //};
});

builder.Services.AddAuthorization();

var app = builder.Build();

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