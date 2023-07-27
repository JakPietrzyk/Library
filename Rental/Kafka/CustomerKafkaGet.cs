using Rental.Dtos;

namespace Rental.Kafka
{
    public class CustomerKafkaGet
    {
        public string Action{get;set;} = "GET";
        public List<CustomerDto>? customerDto {get;set;} = new List<CustomerDto>();
        // public string? Name{get;set;}
        // public string? Surname{get;set;}
        // public ICollection<RentDto> Rents {get;set;} = new List<RentDto>();
    }
}