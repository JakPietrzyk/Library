using System.ComponentModel.DataAnnotations;

namespace BooksLibrary.Dtos
{
    public class CreateBookDto
    {
        [Required]
        public string Title { get; set; }
        [Required]

        public string Author { get; set; }
        [Required]
        
        public DateTimeOffset   Releasedate { get; set; }
    }
}