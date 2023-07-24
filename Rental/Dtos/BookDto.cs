using System.ComponentModel.DataAnnotations;

namespace Rental.Dtos
{
    public class BookDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        // [DataType(DataType.DateTimeOffset)]
        public DateTimeOffset Releasedate { get; set; }
    }
}