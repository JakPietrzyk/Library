using System.Net;
using AutoFixture;
using AutoMapper;
using Rental.Controllers;
using Rental.Dtos;
using Rental.Mappers;
using Rental.Model;
using Rental.Services;
using Rental.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Xunit;
using Rental.Exceptions;

namespace Rental{
    public class Testclass
    {
        private readonly Fixture _fixture;
        private RentalController? _controller; 
        private Mock<IRentalService> _libraryService;
        private Mock<IBooksClient> _booksClientMocked;
        public Testclass()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _libraryService = new Mock<IRentalService>();
            _booksClientMocked = new Mock<IBooksClient>();
        }
        
        [Fact]
        public async Task GetAll_ReturnsOK()
        {
            var customerList = _fixture.CreateMany<CustomerDto>(3).ToList();

            _libraryService.Setup(c => c.GetAll(null, null)).Returns(Task.FromResult<IEnumerable<CustomerDto>>(customerList));

            _controller = new RentalController(_libraryService.Object, _booksClientMocked.Object);

            var result = await _controller.GetAll();
            
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public void GetRent_id_ReturnsOk()
        {
            var customerId = _fixture.Create<int>();
            _fixture.Customize<Customer>(c => c.With(b => b.Id, customerId));
            var customer = _fixture.Create<Customer>();

            _libraryService.Setup(c => c.CheckRent(customerId)).Returns(customer);

            _controller = new RentalController(_libraryService.Object, _booksClientMocked.Object);

            var result = _controller.GetRent(customerId);

            Assert.NotNull(result);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task Create_CreateCustomer_ReturnsCreated()
        {
            var createCustomerDto = _fixture.Create<CreateCustomerDto>();
            int createdId = new();

            _libraryService.Setup(c => c.Create(It.IsAny<Customer>())).Returns(Task.FromResult(createdId));

            _controller = new RentalController(_libraryService.Object, _booksClientMocked.Object);

            var result = await _controller.RentBook(createdId, createCustomerDto);
            
            Assert.NotNull(result);
            var createResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(201, createResult.StatusCode);
        }
        [Fact]
        public async Task Delete_id_ReturnsNoContent()
        {
            var customerId = _fixture.Create<int>();
            var customer = _fixture.Create<CustomerDto>();

            _libraryService.Setup(c => c.Delete(customerId));

            _controller = new RentalController(_libraryService.Object, _booksClientMocked.Object);

            var result = await _controller.DeleteRent(customerId);
            

            Assert.NotNull(result);
            var okResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, okResult.StatusCode); 
        }
        [Fact]
        public async Task RentBook_id_databaseCount_is_1()
        {
            var customer = _fixture.Create<Customer>();

            var dbContextOptions = new DbContextOptionsBuilder<RentalContext>()
                .UseInMemoryDatabase(databaseName: "TestCustomerCreate")
                .Options;

            using(var dbContext = new RentalContext(dbContextOptions))
            {
                var httpClientMock = new Mock<HttpClient>();
                var loggerMock = new Mock<ILogger<RentalService>>();

                var myProfile = new RentalMappingProfile();
                var configuration = new MapperConfiguration(c => c.AddProfile(myProfile));
                IMapper mapper = new Mapper(configuration);


                var service = new RentalService(dbContext, mapper, loggerMock.Object,_booksClientMocked.Object);

                await service.Create(customer);

                Assert.Equal(1, dbContext.Customer.Count());
            }
        }
        [Fact]
        public async void Delete_ExistingCustomer_ReturnsOk_HttpServiceRunning()
        {
            var id = _fixture.Create<int>(); 

            var dbContextOptions = new DbContextOptionsBuilder<RentalContext>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDelete")
                .Options;

            using (var dbContext = new RentalContext(dbContextOptions))
            {
                var existingCustomer = new Customer { 
                    Id = id,
                    Name = "Name",
                    Surname = "Surname",
                    Rents = new List<Rent>(){
                                new Rent(){
                                    Id = id + 1, 

                                }
                    }
                };
                dbContext.Customer.Add(existingCustomer);
                dbContext.SaveChanges();

                var httpClientMock = new Mock<HttpClient>();
                var loggerMock = new Mock<ILogger<RentalService>>();

                var myProfile = new RentalMappingProfile();
                var configuration = new MapperConfiguration(c => c.AddProfile(myProfile));
                IMapper mapper = new Mapper(configuration);
                HttpClient client = new HttpClient();
                var service = new RentalService(dbContext, mapper, loggerMock.Object, _booksClientMocked.Object);

                await service.Delete(id);

                var deletedBook = dbContext.Customer.FirstOrDefault(b => b.Id == id);
                Assert.Null(deletedBook);
            }
        }
         [Fact]
        public async Task Delete_ExistingBook_ReturnsTrue()
        {
            var id = _fixture.Create<int>(); 

            var dbContextOptions = new DbContextOptionsBuilder<RentalContext>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDelete2")
                .Options;

            var dbContext = new RentalContext(dbContextOptions);
            
            var existingCustomer = new Customer { 
                    Id = id,
                    Name = "Name",
                    Surname = "Surname",
                    Rents = new List<Rent>(){
                                new Rent(){
                                    Id = id + 1, 

                                }
                    }
                };
            dbContext.Customer.Add(existingCustomer);
            dbContext.SaveChanges();
        
            var httpClientHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            var httpClient = new HttpClient(httpClientHandlerMock.Object)
            {
                BaseAddress = new Uri("http://example.com") 
            };

            var loggerMock = new Mock<ILogger<RentalService>>();
            var mapperMock = new Mock<IMapper>();
            var myLibraryService = new RentalService(dbContext, mapperMock.Object, loggerMock.Object, _booksClientMocked.Object);

            await myLibraryService.Delete(id);

            Assert.Null(dbContext.Customer.FirstOrDefault(c => c.Id == id));
        }
        [Fact]
        public async Task Delete_NonExistingCustomer_ThrowsNotFoundException()
        {
            var id = _fixture.Create<int>();
            var dbContextOptions = new DbContextOptionsBuilder<RentalContext>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDelete3")
                .Options;

            using (var dbContext = new RentalContext(dbContextOptions))
            {
                var loggerMock = new Mock<ILogger<RentalService>>();
                var mapperMock = new Mock<IMapper>();

                var rentalService = new RentalService(dbContext, mapperMock.Object, loggerMock.Object, _booksClientMocked.Object);

                await Assert.ThrowsAsync<NotFoundException>(() => rentalService.Delete(id));
            }
        }
        [Fact]
        public async Task Rent_BookNotAvailable_ThrowsNotFoundException()
        {
            var myProfile = new RentalMappingProfile();
            var configuration = new MapperConfiguration(c => c.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            var id = _fixture.Create<int>();
            var createCustomerDto = _fixture.Create<CreateCustomerDto>();
            // createCustomerDto.book_id = id+1;
            var rentedBook = _fixture.Create<Book>();
            rentedBook.Id = id + 1;

            var dbContextOptions = new DbContextOptionsBuilder<RentalContext>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDelete3")
                .Options;

            using (var dbContext = new RentalContext(dbContextOptions))
            {
                var loggerMock = new Mock<ILogger<RentalService>>();
                

                var rentalService = new RentalService(dbContext, mapper, loggerMock.Object, _booksClientMocked.Object);
                await rentalService.Rent(createCustomerDto, rentedBook);
                await Assert.ThrowsAsync<NotFoundException>(() => rentalService.Rent(createCustomerDto, rentedBook));
            }
        }
    }
}


