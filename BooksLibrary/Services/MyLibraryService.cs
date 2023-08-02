using BooksLibrary.Model;
using BooksLibrary.Dtos;
using BooksLibrary.Mappers;
using BooksLibrary.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Services
{
    public interface ILibraryService
    {
        Task<BookDto> GetById(int id, HttpContext context);
        BookDto GetByTitle(string title, HttpContext context);
        Task<IEnumerable<BookDto>> GetAll(HttpContext context);
        Task<int> Create(CreateBookDto dto, HttpContext context);
        Task<bool> Delete(int id, HttpContext context);
        Task Update(int id, UpdateBookDto dto, HttpContext context);
        void AddXRequestId(HttpContext context);
    }

    public class MyLibraryService: ILibraryService
    {
        private readonly MyLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MyLibraryService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private string _requestId;
        public MyLibraryService(MyLibraryContext context, IMapper mapper, ILogger<MyLibraryService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
            _url = "http://localhost:5024/api/rental/";
        }
        public void AddXRequestId(HttpContext context)
        {
            var listToUpdate = (List<string>?)context.Items["X-Request-ID"];
            _requestId =  listToUpdate.Last();
        }
        public async Task<BookDto> GetById(int id, HttpContext context)
        {
            AddXRequestId(context);
            _logger.LogDebug($"GET book with id: {id} action invoked");
            var book = await _context.Books.FindAsync(id);

            if(book is null) throw new NotFoundException("Book not found");

            var result = _mapper.Map<BookDto>(book); 
            _logger.LogDebug($"GET book with id: {id} action executed");
            return result;
        }
        public BookDto GetByTitle(string title, HttpContext context)
        {
            AddXRequestId(context);
            _logger.LogDebug($"GET book with title: \"{title}\" action invoked");
            var book = _context.Books.FirstOrDefault(b => b.Title == title);

            if(book is null) throw new NotFoundException("Book not found");

            var result = _mapper.Map<BookDto>(book);
            _logger.LogDebug($"GET book with title: \"{title}\" action executed");
            return result;
        }

        public async Task<IEnumerable<BookDto>> GetAll(HttpContext context)
        {
            AddXRequestId(context);
            _logger.LogDebug($"GET all action invoked");

            // var books = _context.Books.ToList();
            var books = await _context.Books.ToListAsync();

            var booksDtos = _mapper.Map<List<BookDto>>(books);
            _logger.LogDebug($"GET all action executed");
            return booksDtos;
        }

        public async Task<int> Create(CreateBookDto dto, HttpContext context)
        {
            AddXRequestId(context);
            _logger.LogDebug($"Book {dto.Title} CREATE action invoked");
            var book = _mapper.Map<Book>(dto);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            _logger.LogDebug($"Book {dto.Title} CREATE action executed");
            return book.Id;
        }

        public async Task<bool> Delete(int id, HttpContext context)
        {
            AddXRequestId(context);
            _logger.LogDebug($"Book with id: {id} DELETE action invoked");
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if(book is null) throw new NotFoundException("Book not found");

            HttpResponseMessage response;
            try
            {
                if (_httpClient.DefaultRequestHeaders.Contains("X-Request-ID")) _httpClient.DefaultRequestHeaders.Remove("X-Request-ID");
                _httpClient.DefaultRequestHeaders.Add("X-Request-ID", _requestId);
                response = await _httpClient.GetAsync($"{_url}{book.Id}");
            }
            catch
            {
                _logger.LogError($"Rental service is unavaliable!");
                throw new BadHttpRequestException("Can not connect to Rental service");
            }

            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                _logger.LogDebug($"Book with id: {id} DELETE action executed");
                return true;
            }
            throw new BadHttpRequestException("Book is rented");
        }

        public async Task Update(int id, UpdateBookDto dto, HttpContext context)
        {
            AddXRequestId(context);
            _logger.LogDebug($"Book with id: {id} UPDATE action invoked");
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if(book is null) throw new NotFoundException("Book not found");
            if(dto.Title is not null) book.Title = dto.Title;
            if(dto.Author is not null) book.Author = dto.Author;
            if(dto.Releasedate != DateTimeOffset.Parse("0001-01-01T00:00:00+00:00")) book.Releasedate = dto.Releasedate;

            await _context.SaveChangesAsync();
            _logger.LogDebug($"Book with id: {id} UPDATE action executed");
        }
    }
}