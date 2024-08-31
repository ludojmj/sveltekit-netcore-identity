using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Server.Controllers;
using Server.DbModels;
using Server.Services;
using Server.Services.Interfaces;
using Server.Shared;

var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;
var env = builder.Environment;

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ErrorHandler>();
builder.Services.AddHealthChecks();
builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConfiguration(conf.GetSection("Logging")));
builder.Services.AddApplicationInsightsTelemetry();

// Add DB
builder.Services.AddDbContext<StuffDbContext>(options => options.UseSqlite(
    conf.GetConnectionString("SqlConnectionString"),
    sqlServerOptions => sqlServerOptions.CommandTimeout(conf.GetSection("ConnectionStrings:SqlCommandTimeout").Get<int>()))
);

// Add Authent
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = conf["JwtToken:Authority"];
        options.Audience = conf["JwtToken:Audience"];
    });
builder.Services.AddAuthorization();

builder.Services.AddHsts(configureOptions =>
{
    configureOptions.Preload = true;
    configureOptions.IncludeSubDomains = true;
    configureOptions.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});

// Register the Swagger generator
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Server", Version = "v1" });
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStuffService, StuffService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpHeaders();
app.UseExceptionHandler(_ => { });
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseFileServer(new FileServerOptions
{
    EnableDirectoryBrowsing = false,
    EnableDefaultFiles = true,
    DefaultFilesOptions = { DefaultFileNames = { "index.html" } }
});
if (!env.IsProduction())
{
    app.UseSwaggerUI(c =>
    {
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}

if (env.IsDevelopment())
{
    app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}
else
{
    var corsList = conf.GetSection("AuthCors").Get<string[]>();
    app.UseCors(corsPolicyBuilder => corsPolicyBuilder
        .WithOrigins(corsList)
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
}

app.UseAuthentication();
app.UseAuthorization();
app.MapRoutes();
app.MapSwagger();
app.MapFallbackToFile("index.html");
app.MapHealthChecks("/health");

await app.RunAsync();
