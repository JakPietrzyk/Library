using System.ComponentModel.DataAnnotations;

namespace Rental.Dtos
{
    public class CreateCustomerDto
    {
        [Required]
        public string? Name{get;set;}
        [Required]
        public string? Surname{get;set;}
        public ICollection<RentDto> Rents{get;set;} = new List<RentDto>();
    }
}