using HistoryRental.Model;
using HistoryRental.Dtos;
using HistoryRental.Mappers;
using AutoMapper;
using HistoryRental.Exceptions;
using Confluent.Kafka;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace HistoryRental.Services
{
    public interface IHistoryRentalService
    {
        Task<List<MongoDbRental>> GetAll();
        Task<string> Create(MongoDbRental dto);
    }

    public class HistoryRentalService: IHistoryRentalService
    {
        private readonly IMongoCollection<MongoDbRental> _rentalCollection;
        public HistoryRentalService(IOptions<HistoryRentalDatabaseSettings> historyRentalDatabaseSettings)
        {
            // var mongoClient = new MongoClient(historyRentalDatabaseSettings.Value.ConnectionString);
            // var mongoDatabase = mongoClient.GetDatabase(historyRentalDatabaseSettings.Value.DatabaseName);

            // _rentalCollection = mongoDatabase.GetCollection<MongoDbRental>(historyRentalDatabaseSettings.Value.HistoryRentalName);

            var mongoConnectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(mongoConnectionString);
            var database = mongoClient.GetDatabase("RentalHistory");
            _rentalCollection = database.GetCollection<MongoDbRental>("RentalHistory");
        }
        public async Task<List<MongoDbRental>> GetAll()
        {
            return await _rentalCollection.Find(_ => true).ToListAsync();
        }
        public async Task<string> Create(MongoDbRental dto)
        {
            await _rentalCollection.InsertOneAsync(dto);
            return dto.Id;
        }
    }
}