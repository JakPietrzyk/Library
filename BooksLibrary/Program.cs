using BooksLibrary.Model;
using BooksLibrary.Mappers;
using BooksLibrary.Services;
using BooksLibrary.Middleware;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using NLog;



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MyLibraryContext>();
builder.Services.AddAutoMapper(typeof(BookMappingProfile).Assembly);

builder.Services.AddScoped<ILibraryService, MyLibraryService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();

builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


builder.Configuration.GetConnectionString ("DefaultConnection");

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c=>{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyLibrary");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
