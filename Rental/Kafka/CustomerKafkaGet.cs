using Rental.Dtos;

namespace Rental.Kafka
{
    public class CustomerKafkaGet
    {
        public string Action{get;set;} = "GET";

        public List<CustomerDto>? customerDto {get;set;} = new List<CustomerDto>();
        
    }
}