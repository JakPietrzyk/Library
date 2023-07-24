using System.ComponentModel.DataAnnotations;

namespace BooksLibrary.Dtos
{
    public class BookDto
    {

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public DateTimeOffset   Releasedate { get; set; }
    }
}