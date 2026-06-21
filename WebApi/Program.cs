using DataAccess;
using DataAccess.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDatabaseContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ReadCity API",
        Version = "v1"
    });

    foreach (var xmlFileName in new[]
             {
                 $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
                 $"{typeof(AppDatabaseContext).Assembly.GetName().Name}.xml"
             })
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "DesktopClient",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var isDatabaseFailure = ContainsException<SqlException>(exception);

        context.Response.StatusCode = isDatabaseFailure
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = isDatabaseFailure ? "Database unavailable" : "Unexpected server error",
            Detail = isDatabaseFailure
                ? "The API could not reach SQL Server. Check the local SQL Server instance and the DefaultConnection string."
                : "The server could not process the request."
        });
    });
});
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ReadCity API v1");
    options.DocumentTitle = "ReadCity API";
});

app.UseCors("DesktopClient");
app.MapControllers();
app.Run();

static bool ContainsException<TException>(Exception? exception)
    where TException : Exception
{
    while (exception is not null)
    {
        if (exception is TException)
        {
            return true;
        }

        exception = exception.InnerException;
    }

    return false;
}
