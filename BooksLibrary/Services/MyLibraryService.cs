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
        Task<BookDto> GetById(int id);
        BookDto GetByTitle(string title);
        Task<IEnumerable<BookDto>> GetAll();
        Task<int> Create(CreateBookDto dto);
        Task<bool> Delete(int id);
        Task Update(int id, UpdateBookDto dto);
    }

    public class MyLibraryService: ILibraryService
    {
        private readonly MyLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MyLibraryService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _url;
        public MyLibraryService(MyLibraryContext context, IMapper mapper, ILogger<MyLibraryService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
            _url = "http://localhost:5024/api/rental/";


        }

        public async Task<BookDto> GetById(int id)
        {
            _logger.LogInformation($"GET book with id: {id} action invoked");
            var book = await _context.Books.FindAsync(id);

            if(book is null) throw new NotFoundException("Book not found");

            var result = _mapper.Map<BookDto>(book); 
            _logger.LogInformation($"GET book with id: {id} action executed");
            return result;
        }
        public BookDto GetByTitle(string title)
        {
            _logger.LogInformation($"GET book with title: \"{title}\" action invoked");
            var book = _context.Books.FirstOrDefault(b => b.Title == title);

            if(book is null) throw new NotFoundException("Book not found");

            var result = _mapper.Map<BookDto>(book);
            _logger.LogInformation($"GET book with title: \"{title}\" action executed");
            return result;
        }

        public async Task<IEnumerable<BookDto>> GetAll()
        {
            _logger.LogInformation($"GET all action invoked");

            // var books = _context.Books.ToList();
            var books = await _context.Books.ToListAsync();

            var booksDtos = _mapper.Map<List<BookDto>>(books);
            _logger.LogInformation($"GET all action executed");
            return booksDtos;
        }

        public async Task<int> Create(CreateBookDto dto)
        {
            _logger.LogInformation($"Book {dto.Title} CREATE action invoked");
            var book = _mapper.Map<Book>(dto);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Book {dto.Title} CREATE action executed");
            return book.Id;
        }

        public async Task<bool> Delete(int id)
        {
            _logger.LogInformation($"Book with id: {id} DELETE action invoked");
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if(book is null) throw new NotFoundException("Book not found");

            HttpResponseMessage response;
            try
            {
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
                _logger.LogInformation($"Book with id: {id} DELETE action executed");
                return true;
            }
            throw new BadHttpRequestException("Book is rented");
        }

        public async Task Update(int id, UpdateBookDto dto)
        {
            _logger.LogInformation($"Book with id: {id} UPDATE action invoked");
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if(book is null) throw new NotFoundException("Book not found");
            if(dto.Title is not null) book.Title = dto.Title;
            if(dto.Author is not null) book.Author = dto.Author;
            if(dto.Releasedate != DateTimeOffset.Parse("0001-01-01T00:00:00+00:00")) book.Releasedate = dto.Releasedate;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Book with id: {id} UPDATE action executed");
        }
    }
}