using System.ComponentModel.DataAnnotations;

namespace Rental.Dtos
{
    public class CustomerDto
    {
        [Required]
        public string Name{get;set;}
        [Required]
        public string Surname{get;set;}
        public DateTimeOffset Rental_date{get;set;}
        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public DateTimeOffset Releasedate { get; set;}
    }
}