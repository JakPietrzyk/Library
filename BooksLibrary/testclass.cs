using System.Net;
using AutoFixture;
using AutoMapper;
using BooksLibrary.Controllers;
using BooksLibrary.Dtos;
using BooksLibrary.Mappers;
using BooksLibrary.Model;
using BooksLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Xunit;


namespace BooksLibrary{
    public class Testclass
    {
        private readonly Fixture _fixture;
        private MyLibraryController? _controller; 
        private Mock<ILibraryService> _libraryService;
        public Testclass()
        {
            _fixture = new Fixture();
            _libraryService = new Mock<ILibraryService>();
        }
        [Fact]
        public async Task Get_BooksDto_ReturnOk()
        {
            var booksList = _fixture.CreateMany<BookDto>(3).ToList();

            _libraryService.Setup(c => c.GetAll()).Returns(Task.FromResult<IEnumerable<BookDto>>(booksList));

            _controller = new MyLibraryController(_libraryService.Object);

            var result = await _controller.GetAll();
            
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task GetById_Book_ReturnOk()
        {
            var bookId = 3;
            _fixture.Customize<BookDto>(c => c.With(b => b.Id, bookId));
            var book = _fixture.Create<BookDto>();

            _libraryService.Setup(c => c.GetById(bookId)).Returns(Task.FromResult(book));

            _controller = new MyLibraryController(_libraryService.Object);

            var result = await _controller.Get(bookId);

            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task Post_BookDto_ReturnCreated()
        {
            var createBookDto = _fixture.Create<CreateBookDto>();
            int createdId = new();

            _libraryService.Setup(c => c.Create(It.IsAny<CreateBookDto>())).Returns(Task.FromResult(createdId));

            _controller = new MyLibraryController(_libraryService.Object);

            var result = await _controller.CreateBook(createBookDto);
            
            Assert.NotNull(result);
            var createResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createResult.StatusCode);
        }
        [Fact]
        public async Task Put_BookDto_ReturnOk()
        {
            var bookId = 3;
            var book = _fixture.Create<UpdateBookDto>();

            _libraryService.Setup(c => c.Update(It.IsAny<int>(), It.IsAny<UpdateBookDto>()));

            _controller = new MyLibraryController(_libraryService.Object);

            var result = await _controller.Put(bookId, book);
            
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }


        [Fact]
        public async Task Delete_ValidId_ReturnsTrue()
        {
            var bookId = 3;
            var book = _fixture.Create<UpdateBookDto>();

            _libraryService.Setup(c => c.Delete(bookId));

            _controller = new MyLibraryController(_libraryService.Object);

            var result = await _controller.Delete(bookId);
            

            Assert.NotNull(result);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode); 
        }
        [Fact]
        public async Task Create_BookDto_ReturnOk()
        {
            var book = _fixture.Create<CreateBookDto>();

            var dbContextOptions = new DbContextOptionsBuilder<MyLibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestBooksCreate")
                .Options;

            using(var dbContext = new MyLibraryContext(dbContextOptions))
            {
                var httpClientMock = new Mock<HttpClient>();
                var loggerMock = new Mock<ILogger<MyLibraryService>>();

                var myProfile = new BookMappingProfile();
                var configuration = new MapperConfiguration(c => c.AddProfile(myProfile));
                IMapper mapper = new Mapper(configuration);

                var service = new MyLibraryService(dbContext, mapper, loggerMock.Object, httpClientMock.Object);

                await service.Create(book);

                Assert.Equal(1, dbContext.Books.Count());
            }
        }
        [Fact]
        public async void Update_ExistingBook_ReturnsOk()
        {
            var id = _fixture.Create<int>(); 
            var updatedBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Releasedate = DateTime.UtcNow
            };

            var dbContextOptions = new DbContextOptionsBuilder<MyLibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestBooksUpdate")
                .Options;

            using (var dbContext = new MyLibraryContext(dbContextOptions))
            {
                var existingBook = new Book { 
                    Id = id, 
                    Title = "Old Title", 
                    Author = "Old Author", 
                    Releasedate = DateTime.UtcNow 
                };
                dbContext.Books.Add(existingBook);
                dbContext.SaveChanges();

                var httpClientMock = new Mock<HttpClient>();
                var loggerMock = new Mock<ILogger<MyLibraryService>>();

                var myProfile = new BookMappingProfile();
                var configuration = new MapperConfiguration(c => c.AddProfile(myProfile));
                IMapper mapper = new Mapper(configuration);

                var service = new MyLibraryService(dbContext, mapper, loggerMock.Object, httpClientMock.Object);

                await service.Update(id, updatedBookDto);

                var updatedBook = dbContext.Books.FirstOrDefault(b => b.Id == id);
                Assert.NotNull(updatedBook);
                Assert.Equal(updatedBookDto.Title, updatedBook.Title);
                Assert.Equal(updatedBookDto.Author, updatedBook.Author);
                Assert.Equal(updatedBookDto.Releasedate, updatedBook.Releasedate); 
            }
        }
        [Fact]
        public async void Delete_ExistingBook_ReturnsOk_HttpServiceRunning()
        {
            var id = _fixture.Create<int>(); 

            var dbContextOptions = new DbContextOptionsBuilder<MyLibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestBooksDelete")
                .Options;

            using (var dbContext = new MyLibraryContext(dbContextOptions))
            {
                var existingBook = new Book { 
                    Id = id, 
                    Title = "Title", 
                    Author = "Author", 
                    Releasedate = DateTime.UtcNow 
                };
                dbContext.Books.Add(existingBook);
                dbContext.SaveChanges();

                var httpClientMock = new Mock<HttpClient>();
                var loggerMock = new Mock<ILogger<MyLibraryService>>();

                var myProfile = new BookMappingProfile();
                var configuration = new MapperConfiguration(c => c.AddProfile(myProfile));
                IMapper mapper = new Mapper(configuration);
                HttpClient client = new HttpClient();
                var service = new MyLibraryService(dbContext, mapper, loggerMock.Object, client);

                await service.Delete(id);

                var deletedBook = dbContext.Books.FirstOrDefault(b => b.Id == id);
                Assert.Null(deletedBook);
            }
        }
        [Fact]
        public async Task Delete_ExistingBook_ReturnsTrue()
        {
            var id = _fixture.Create<int>(); 

            var dbContextOptions = new DbContextOptionsBuilder<MyLibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestBooksDelete2")
                .Options;

            var dbContext = new MyLibraryContext(dbContextOptions);
            
            var existingBook = new Book { Id = id, Title = "Existing Title", Author = "Existing Author", Releasedate = DateTime.UtcNow };
            dbContext.Books.Add(existingBook);
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

            var loggerMock = new Mock<ILogger<MyLibraryService>>();
            var mapperMock = new Mock<IMapper>();
            var myLibraryService = new MyLibraryService(dbContext, mapperMock.Object, loggerMock.Object, httpClient);

            var result = await myLibraryService.Delete(id);

            Assert.True(result);
        }
    }
}



