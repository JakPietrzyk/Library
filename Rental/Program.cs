using Rental.Model;
using Rental.Services;
using Rental.Clients;
using Rental.Mappers;
using Rental.Middleware;
using Rental.Loggers;
using NLog.Web;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Layouts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<RentalContext>();
builder.Services.AddAutoMapper(typeof(RentalMappingProfile).Assembly);

builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IBooksClient, BooksClient>();

builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<LoggerFilterAttribbute>();
// builder.Services.AddMvc(o => o.Filters.Add<LoggerFilterAttribbute>());

builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<BooksClient>();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);

// NLog Configuration
NLogJsonConfiguration.Configure();

var logger = LogManager.GetCurrentClassLogger();

// End of NLog Configuration

builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rental");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
