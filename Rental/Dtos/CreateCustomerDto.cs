using System.ComponentModel.DataAnnotations;

namespace Rental.Dtos
{
    public class CreateCustomerDto
    {
        [Required]
        public string? Name{get;set;}
        [Required]
        public string? Surname{get;set;}
        public DateTimeOffset Rental_date{get;set;}
        public int book_id{get;set;}
        public string? Title { get; set; }

        public string? Author { get; set; }

        public DateTimeOffset Releasedate { get; set;}
    }
}