using System.Net;
using AutoFixture;
using AutoMapper;
using HistoryRental.Controllers;
using HistoryRental.Dtos;
using HistoryRental.Mappers;
using HistoryRental.Model;
using HistoryRental.Services;
using HistoryRental.Clients;
using HistoryRental.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using Xunit;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using HistoryRental.Settings;
using MongoDB.Bson;

namespace Rental
{
    // public class MongoDbServiceUnitTest
    // {
    //     private Mock <IMongoClient> _mongoClient;
    //     private Mock <IMongoDatabase> _mongodb;
    //     private Mock <IMongoCollection<MongoDbRental>> _dataCollection;
    //     private List <MongoDbRental> _dataList;
    //     private HistoryRentalDatabaseSettings _historyRentalDatabaseSettings;
    //     private readonly Fixture _fixture;
    //     public MongoDbServiceUnitTest()
    //     {


    //         _mongoClient = new Mock<IMongoClient>();
    //         _dataCollection = new Mock<IMongoCollection<MongoDbRental>>();
    //         _mongodb = new Mock<IMongoDatabase>();

    //         _dataList = _fixture.CreateMany<MongoDbRental>(7).ToList();
    //     }
    //     private void InitializeMongoDb() {
    //         _mongodb.Setup(x => x.GetCollection < MongoDbRental > (_historyRentalDatabaseSettings.RentalCollectionName,
    //             default)).Returns(_dataCollection.Object);
    //         _mongoClient.Setup(x => x.GetDatabase(It.IsAny < string > (),
    //             default)).Returns(_mongodb.Object);
    //     }
    // }

    public class Testclass
    {
        private Mock <IMongoClient> _mongoClient;
        private Mock <IMongoDatabase> _mongodb;
        private Mock <IMongoCollection<MongoDbRental>> _dataCollection;
        private List <MongoDbRental> _dataList;
        private HistoryRentalDatabaseSettings _historyRentalDatabaseSettings;
        private readonly Fixture _fixture;
        private readonly IConfigurationRoot _configuration;
        private RentalController? _controller; 
        private Mock<IHistoryRentalService> _libraryService;
        private Mock<IBooksClient> _booksClientMocked;
        private Mock<IRentalClient> _rentalClientMocked;
        private Mock<ILogger<HistoryRentalService>> _loggerMocked;
        private Mock<IMapper> _mapper;
        public Testclass()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _libraryService = new Mock<IHistoryRentalService>();
            _booksClientMocked = new Mock<IBooksClient>();
            _rentalClientMocked = new Mock<IRentalClient>();
            _loggerMocked = new Mock<ILogger<HistoryRentalService>>();
            _mongoClient = new Mock<IMongoClient>();
            _dataCollection = new Mock<IMongoCollection<MongoDbRental>>();
            _mongodb = new Mock<IMongoDatabase>();
            _mapper = new Mock<IMapper>();
            _dataList = _fixture.CreateMany<MongoDbRental>(7).ToList();
            _historyRentalDatabaseSettings = new HistoryRentalDatabaseSettings();

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Set the base path to the test project's output directory
                .AddJsonFile("C:/Users/Jakub/Documents/C#/1 task/HistoryRental/appsettings.json", optional: true, reloadOnChange: true); // Load appsettings.json from the "Configs" folder

            _configuration = configurationBuilder.Build();
        }
        private void InitializeMongoDb() {
            _mongodb.Setup(x => x.GetCollection < MongoDbRental > (_historyRentalDatabaseSettings.RentalCollectionName,
                default)).Returns(_dataCollection.Object);
            _mongoClient.Setup(x => x.GetDatabase(It.IsAny < string > (),
                default)).Returns(_mongodb.Object);

        }
        private void InitializeMongoProductCollection() 
        {
            ClearDatabase();
            this.InitializeMongoDb();
  
        }
        private void ClearDatabase()
        {
            var connectionString = _configuration["HistoryDatabaseTesting:ConnectionString"];
            var client = new MongoClient(connectionString);
            var databaseName = _configuration["HistoryDatabaseTesting:DatabaseName"];
            var collectionName = _configuration["HistoryDatabaseTesting:RentalCollectionName"];

            // Get the database and the collection
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            // Delete all documents from the collection
            collection.DeleteMany(new BsonDocument());
        }
        [Fact]
        public async Task GetAll_ReturnsOK()
        {
            int customerId = 1;
            int n = 5;
            var customerList = _fixture.Create<CustomerDto>();
            var optionsMongo = new Mock<IOptions<HistoryRentalDatabaseSettings>>();

            var booksClientMock = new Mock<IBooksClient>();
            booksClientMock.Setup(c => c.GetBook(It.IsAny<int>()))
                .Returns(Task.FromResult(new Book { Title = "Sample Book", Author = "John Smith" }));
            var rentalClientMock = new Mock<IRentalClient>();
            rentalClientMock.Setup(c => c.GetCustomer(customerId))
                .ReturnsAsync(new CustomerDto { Name = "John", Surname = "Doe" });

            var mapperMock = new Mock<IMapper>();
            _libraryService.Setup(c => c.GetAll(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(customerList));
            _libraryService.Setup(c => c.AddXRequestId(It.IsAny<HttpContext>()));
            var historyRentalService = new Mock<HistoryRentalService>(
                optionsMongo.Object,
                booksClientMock.Object,
                rentalClientMock.Object,
                mapperMock.Object,
                _loggerMocked.Object
            );
            _controller = new RentalController(_libraryService.Object);

            var result = await _controller.GetAll(customerId,n);
            
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
         [Fact]
        public async Task GetAll_Should_Return_CustomerDto()
        {
            int customerId = 1;
            int n = 5;

            var rentalClientMock = new Mock<IRentalClient>();
            rentalClientMock.Setup(c => c.GetCustomer(customerId))
                .ReturnsAsync(new CustomerDto { Name = "John", Surname = "Doe" });
            var optionsMongo = new Mock<IOptions<HistoryRentalDatabaseSettings>>();
            optionsMongo.Setup(x => x.Value).Returns(new HistoryRentalDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "RentalHistoryTesting",
                RentalCollectionName = "RentalHistoryTesting" 
            });
            var booksClientMock = new Mock<IBooksClient>();
            booksClientMock.Setup(c => c.GetBook(It.IsAny<int>()))
                .Returns(Task.FromResult(new Book { Title = "Sample Book", Author = "John Smith" }));

            var mapperMock = new Mock<IMapper>();

            var historyRentalService = new HistoryRentalService(
                optionsMongo.Object,
                booksClientMock.Object,
                rentalClientMock.Object,
                mapperMock.Object,
                _loggerMocked.Object
                // _mongoClient.Object
            );

            var result = await historyRentalService.GetAll(customerId, n);

            Assert.NotNull(result);
            Assert.Equal("John", result.Name);
            Assert.Equal("Doe", result.Surname);
        }
        [Fact]
        public async Task GetAll_Should_Return_5_Rents()
        {
            var cusotmerId = 1;
            InitializeMongoProductCollection();
            var optionsMongo = new Mock<IOptions<HistoryRentalDatabaseSettings>>();
            optionsMongo.Setup(x => x.Value).Returns(new HistoryRentalDatabaseSettings
            {
                ConnectionString = _configuration["HistoryDatabaseTesting:ConnectionString"],
                DatabaseName = _configuration["HistoryDatabaseTesting:DatabaseName"],
                RentalCollectionName = _configuration["HistoryDatabaseTesting:RentalCollectionName"]
            });
            _rentalClientMocked.Setup(c => c.GetCustomer(cusotmerId)).Returns(Task.FromResult(new CustomerDto { Name = "John", Surname = "Doe" }));
            var mongoDBService = new HistoryRentalService(optionsMongo.Object,_booksClientMocked.Object,_rentalClientMocked.Object,_mapper.Object,_loggerMocked.Object);
            var listToAdd = _fixture.CreateMany<MongoDbRental>(7).ToList();
            foreach(var rent in listToAdd)
            {
                rent.Id = ObjectId.GenerateNewId().ToString();
                rent.cusotmerId = cusotmerId;
                await mongoDBService.Create(rent);
            }
            var response = await mongoDBService.GetAll(cusotmerId,5);
            Assert.Equal(response.Rents.Count, 5);
            
        }
    }
}



