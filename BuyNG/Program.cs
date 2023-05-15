using AspNetCoreRateLimit;
using BuyNG.Core;
using BuyNG.Core.Configuration;
using BuyNG.Core.IRepository;
using BuyNG.Core.Models;
using BuyNG.Core.Repository;
using BuyNG.Core.Services;
using BuyNG.Data;
using BuyNG.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"));
});

builder.Services.AddAuthentication();

builder.Services.ConfigureIdentity();

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.WriteTo.File("C:\\Users\\HP\\Desktop\\C# project\\BuyNG\\BuyNG-.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
});

builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// add unit of work as service

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IEmailManager, EmailManager>();

builder.Services.Configure<SMTPConfigModel>(builder.Configuration.GetSection("SMTPConfig"));

builder.Services.AddAutoMapper(typeof(MapperIntializer));

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(op =>
    {
        op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddMemoryCache();

builder.Services.ConfigureVersioning();

builder.Services.ConfigureRateLimiting();

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureHttpCacheHeaders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDoc();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseApiVersioning();

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "ProductImages")),
    RequestPath = "/ProductImages"
});

app.UseCors("CorsPolicy");

app.UseResponseCaching();

app.UseHttpCacheHeaders();

app.UseIpRateLimiting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
