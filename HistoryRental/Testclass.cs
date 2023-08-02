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

namespace Rental{
    public class Testclass
    {
        private readonly Fixture _fixture;
        private RentalController? _controller; 
        private Mock<IHistoryRentalService> _libraryService;
        private Mock<IBooksClient> _booksClientMocked;
        private Mock<IRentalClient> _rentalClientMocked;
        private Mock<ILogger<HistoryRentalService>> _loggerMocked;
        public Testclass()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _libraryService = new Mock<IHistoryRentalService>();
            _booksClientMocked = new Mock<IBooksClient>();
            _rentalClientMocked = new Mock<IRentalClient>();
            _loggerMocked = new Mock<ILogger<HistoryRentalService>>();
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

            // historyRentalService.Setup(c => c.AddXRequestId(It.IsAny<HttpContext>()));

            _controller = new RentalController(_libraryService.Object);

            var result = await _controller.GetAll(customerId,n);
            
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
         [Fact]
        public async Task GetAll_Should_Return_CustomerDto()
        {
            // Arrange
            int customerId = 1;
            int n = 5;

            // Mock the dependencies
            var rentalClientMock = new Mock<IRentalClient>();
            rentalClientMock.Setup(c => c.GetCustomer(customerId))
                .ReturnsAsync(new CustomerDto { Name = "John", Surname = "Doe" });

            // var rentalCollectionMock = new Mock<IOptions<MongoDbRental>>();
            // rentalCollectionMock.Setup(c => c.FindAsync(
            //     It.IsAny<FilterDefinition<MongoDbRental>>(),
            //     It.IsAny<FindOptions<MongoDbRental, MongoDbRental>>(),
            //     default // Provide a default CancellationToken
            // ))
            // .Returns(Task.FromResult((IAsyncCursor<MongoDbRental>)new List<MongoDbRental>
            // {
            //     new MongoDbRental { cusotmerId = customerId, returnDate = new DateTime(2023, 8, 10) },
            //     new MongoDbRental { cusotmerId = customerId, returnDate = new DateTime(2023, 8, 12) },
            //     new MongoDbRental { cusotmerId = customerId, returnDate = new DateTime(2023, 8, 15) },
            //     new MongoDbRental { cusotmerId = customerId, returnDate = new DateTime(2023, 8, 20) },
            //     new MongoDbRental { cusotmerId = customerId, returnDate = new DateTime(2023, 8, 25) },
            // }));
            var optionsMongo = new Mock<IOptions<HistoryRentalDatabaseSettings>>();

            var booksClientMock = new Mock<IBooksClient>();
            booksClientMock.Setup(c => c.GetBook(It.IsAny<int>()))
                .Returns(Task.FromResult(new Book { Title = "Sample Book", Author = "John Smith" }));

            var mapperMock = new Mock<IMapper>();

            // Create an instance of the HistoryRentalService with the mocked dependencies
            var historyRentalService = new HistoryRentalService(
                optionsMongo.Object,
                booksClientMock.Object,
                rentalClientMock.Object,
                mapperMock.Object,
                _loggerMocked.Object
            );

            // Act
            var result = await historyRentalService.GetAll(customerId, n);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            // Assert.NotNull(result);
            // Assert.Equal("John", result.Name);
            // Assert.Equal("Doe", result.Surname);
            // Assert.Equal(n, result.Rents.Count);
        }
    }
}



