using Rental.Model;
using Rental.Services;
using Rental.Clients;
using Rental.Mapper;
using Rental.Middleware;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<RentalContext>();
builder.Services.AddAutoMapper(typeof(RentalMappingProfile).Assembly);

builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<BooksClient>();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c=>{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rental");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
