namespace HistoryRental.Settings
{
    public class HistoryRentalDatabaseSettings
    {   
        public HistoryRentalDatabaseSettings()
        {
            Environment.SetEnvironmentVariable("connection-string", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("database-name", "RentalHistory");
        }
        public string ConnectionString {get;set;} = null!;
        public string DatabaseName {get;set;} = null!;
        public string RentalCollectionName{get;set;} = null!;
    }
    
}