using Rental.Dtos;

namespace Rental.Kafka
{
    public class CustomerKafkaGet
    {
        public string Action{get;set;} = "GET";
        public int CusotmerId{get;set;}
        public int BookId{get;set;} 
        public int RentId{get;set;}
    }
}