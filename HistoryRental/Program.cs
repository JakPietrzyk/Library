using HistoryRental.Model;
using HistoryRental.Services;
using HistoryRental.Mappers;
using HistoryRental.Middleware;
using NLog.Web;
using Confluent.Kafka;
using HistoryRental.Clients;
using HistoryRental.Loggers;
using HistoryRental.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var _kafkaConfig = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "listener"
};
builder.Services.AddSingleton<ConsumerConfig>();


builder.Services.AddControllers();
builder.Services.Configure<HistoryRentalDatabaseSettings>(
    builder.Configuration.GetSection("HistoryDatabase")
);
builder.Services.AddScoped<IHistoryRentalService, HistoryRentalService>();
builder.Services.AddScoped<IBooksClient, BooksClient>();
builder.Services.AddScoped<IRentalClient, RentalClient>();
builder.Services.AddAutoMapper(typeof(HistoryRentalMappingProfile).Assembly);

builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<LoggerFilterAttribbute>();

builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<BooksClient>();
builder.Services.AddHttpClient<RentalClient>();
builder.Services.AddHostedService<KafkaMessageService>();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);

NLogJsonConfiguration.Configure();

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
