using BooksLibrary.Model;
using BooksLibrary.Dtos;
using BooksLibrary.Mapper;
using BooksLibrary.Exceptions;
using AutoMapper;

namespace BooksLibrary.Services
{
    public interface ILibraryService
    {
        BookDto GetById(int id);
        BookDto GetByTitle(string title);
        IEnumerable<BookDto> GetAll();
        int Create(CreateBookDto dto);
        void Delete(int id);
        void Update(int id, UpdateBookDto dto);
    }

    public class MyLibraryService: ILibraryService
    {
        private readonly MyLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MyLibraryService> _logger;
        public MyLibraryService(MyLibraryContext context, IMapper mapper, ILogger<MyLibraryService> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public BookDto GetById(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if(book is null) throw new NotFoundException("Book not found");

            var result = _mapper.Map<BookDto>(book); 
            return result;
        }
        public BookDto GetByTitle(string title)
        {
            var book = _context.Books.FirstOrDefault(b => b.Title == title);

            if(book is null) throw new NotFoundException("Book not found");

            var result = _mapper.Map<BookDto>(book);
            return result;
        }

        public IEnumerable<BookDto> GetAll()
        {
            var books = _context.Books.ToList();

            var booksDtos = _mapper.Map<List<BookDto>>(books);
            
            return booksDtos;
        }

        public int Create(CreateBookDto dto)
        {
            var book = _mapper.Map<Book>(dto);
            _context.Books.Add(book);
            _context.SaveChanges();

            return book.Id;
        }

        public void Delete(int id)
        {
            _logger.LogError($"Book with id: {id} DELETE action invoked");
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if(book is null) throw new NotFoundException("Book not found");

            _context.Books.Remove(book);
            _context.SaveChanges();
        }

        public void Update(int id, UpdateBookDto dto)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if(book is null) throw new NotFoundException("Book not found");

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.Releasedate = dto.Releasedate;

            _context.SaveChanges();
        }
    }
}