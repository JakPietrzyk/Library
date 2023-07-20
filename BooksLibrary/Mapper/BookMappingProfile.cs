
using AutoMapper;
using BooksLibrary.Model;
using BooksLibrary.Dtos;

namespace BooksLibrary.Mapper
{
    public class BookMappingProfile: Profile
    {
        public BookMappingProfile()
        {
            CreateMap<Book,BookDto>();

            CreateMap<CreateBookDto,Book>();
        }

    }
}