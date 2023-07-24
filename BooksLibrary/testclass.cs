using AutoFixture;
using AutoMapper;
using BooksLibrary.Controllers;
using BooksLibrary.Dtos;
using BooksLibrary.Model;
using BooksLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;


namespace BooksLibrary{
    public class testclass
    {
        private readonly Fixture _fixture;
        private MyLibraryController? _controller; 
        private Mock<ILibraryService> _libraryService;
        public testclass()
        {
            _fixture = new Fixture();
            _libraryService = new Mock<ILibraryService>();
        }
        [Fact]
        public async Task Get_BooksDto_ReturnOk()
        {
            var booksList = _fixture.CreateMany<BookDto>(3).ToList();

            _libraryService.Setup(c => c.GetAll()).Returns(booksList);

            _controller = new MyLibraryController(_libraryService.Object);

            var result = _controller.GetAll();
            
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

            _libraryService.Setup(c => c.GetById(bookId)).Returns(book);

            _controller = new MyLibraryController(_libraryService.Object);

            var result = _controller.Get(bookId);

            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task Post_BookDto_ReturnCreated()
        {
            var createBookDto = _fixture.Create<CreateBookDto>();
            int createdId = new();

            _libraryService.Setup(c => c.Create(It.IsAny<CreateBookDto>())).Returns(createdId);

            _controller = new MyLibraryController(_libraryService.Object);

            var result = _controller.CreateBook(createBookDto);
            
            Assert.NotNull(result);
            var createResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createResult.StatusCode);
        }
        [Fact]
        public async Task Put_BookDto_ReturnOk()
        {
            var bookId = 3;
            // _fixture.Customize<UpdateBookDto>(c => c.With(b => b.Id, bookId));
            var book = _fixture.Create<UpdateBookDto>();

            _libraryService.Setup(c => c.Update(It.IsAny<int>(), It.IsAny<UpdateBookDto>()));

            _controller = new MyLibraryController(_libraryService.Object);

            var result = _controller.Put(bookId, book);
            
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }


       [Fact]
        public async Task Delete_ValidId_ReturnsTrue()
        {
            var bookId = 3;
            // _fixture.Customize<UpdateBookDto>(c => c.With(b => b.Id, bookId));
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
            // var bookId = 1;
            // var book = _fixture.Create<CreateBookDto>();

            // var dbContextOptions = new DbContextOptionsBuilder<MyLibraryContext>()
            //     .UseInMemoryDatabase(databaseName: "TestBooks").Options;

            // var dbContext = new MyLibraryContext(dbContextOptions);
            // var httpClientMock = new Mock<HttpClient>();
            // var loggerMock = new Mock<ILogger<MyLibraryService>>();
            // var mapper = new Mock<IMapper>();
            // var service = new MyLibraryService(dbContext, mapper.Object, loggerMock.Object, httpClientMock.Object);

            // service.Create(book);

            // Assert.Equal(1, dbContext.Books.Count());
        }
    }
}

