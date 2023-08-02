using AutoMapper;
using Confluent.Kafka;
using HistoryRental.Model;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace HistoryRental.Services
{
    
    public class KafkaMessageService: BackgroundService
    {
        private readonly IMongoCollection<MongoDbRental> _rentalCollection;
        private readonly string _kafkaTopic;
        private readonly ConsumerConfig _consumerConfig;
        private readonly IMapper _mapper;
        private readonly ILogger<KafkaMessageService> _logger;
        public KafkaMessageService(IMapper mapper, ILogger<KafkaMessageService> logger)
        {
            _mapper = mapper;
            var mongoConnectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(mongoConnectionString);
            var database = mongoClient.GetDatabase("RentalHistory");
            _rentalCollection = database.GetCollection<MongoDbRental>("RentalHistory");

            _consumerConfig = new ConsumerConfig{
                BootstrapServers = "localhost:9092",
                GroupId = "listener"
            };
            _kafkaTopic = "rentalEvents";
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => GetMessageFromKafka(stoppingToken));
            return Task.CompletedTask;
        }

        public async Task GetMessageFromKafka(CancellationToken stoppingToken)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                consumer.Subscribe(_kafkaTopic);
                try
                {
                    while(!stoppingToken.IsCancellationRequested)
                    {
                        var message = consumer.Consume(stoppingToken);
                        _logger.LogDebug(($"Data from Kafka recieved {message.Value} invoked"));
                        var result = JsonConvert.DeserializeObject<KafkaRental>(message.Value);
                        if(result is not null && result.ReturnDate is not null)
                        {
                            var filter = Builders<MongoDbRental>.Filter.Where(f => f.RentId == result.RentId);
                            var update = Builders<MongoDbRental>.Update.Set(r => r.returnDate, DateTime.Now);
                            await _rentalCollection.FindOneAndUpdateAsync(filter,update,new FindOneAndUpdateOptions<MongoDbRental>(), stoppingToken);
                        }
                        else
                        {
                            var sendToDatabase = _mapper.Map<MongoDbRental>(result);
                            await _rentalCollection.InsertOneAsync(_mapper.Map<MongoDbRental>(result), cancellationToken: stoppingToken);
                        }
                        _logger.LogDebug(($"Data from Kafka recieved {message.Value} consumed"));
                    }
                }
                catch(OperationCanceledException)
                {
                    consumer.Close();
                }
            }
        }
    }
}

