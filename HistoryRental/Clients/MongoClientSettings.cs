using HistoryRental.Settings;
using MongoDB.Driver;

namespace HistoryRental.Clients
{
    public class MongoClientSettings
    {
        private readonly MongoClient _mongoClient;
        private readonly HistoryRentalDatabaseSettings _historyRentalDatabaseSettings;
        MongoClientSettings(HistoryRentalDatabaseSettings historyRentalDatabaseSettings)
        {
            _historyRentalDatabaseSettings = historyRentalDatabaseSettings;
            _mongoClient = new MongoClient(_historyRentalDatabaseSettings.ConnectionString);
        }
    }
}