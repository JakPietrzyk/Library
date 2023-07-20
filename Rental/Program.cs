using Rental.Model;
using Rental.Services;
using Rental.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<RentalContext>();
builder.Services.AddAutoMapper(typeof(RentalMappingProfile).Assembly);

builder.Services.AddScoped<IRentalService, RentalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
