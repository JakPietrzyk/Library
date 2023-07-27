using Rental.Dtos;

namespace Rental.Kafka
{
    public class CustomerKafkaDelete
    {
        public string Action{get;set;} = "Delete";
        public string? Name{get;set;}
        public string? Surname{get;set;}
        public ICollection<RentDto> Rents {get;set;} = new List<RentDto>();
    }
}