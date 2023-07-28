using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HistoryRental.Dtos
{
    public class CustomerDto
    {
        [Required]
        public string? Name{get;set;}
        [Required]
        public string? Surname{get;set;}
        public ICollection<RentDto> Rents {get;set;} = new List<RentDto>();
    }
}