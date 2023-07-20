using System.ComponentModel.DataAnnotations;

namespace BooksLibrary.Dtos
{
    public class CreateBookDto
    {

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;
        
        public DateTime Releasedate { get; set; }
    }
}