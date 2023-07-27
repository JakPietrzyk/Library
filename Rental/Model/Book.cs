using System.ComponentModel.DataAnnotations;

namespace Rental.Dtos
{
    public class Book
    {
        [Required]
        public int Id{get;set;}
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public DateTimeOffset Releasedate { get; set; }
    }
}