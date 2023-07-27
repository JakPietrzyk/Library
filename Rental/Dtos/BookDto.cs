using System.ComponentModel.DataAnnotations;

namespace Rental.Dtos
{
    public class BookDto
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public DateTime Releasedate { get; set; }
    }
}