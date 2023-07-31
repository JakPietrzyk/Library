using Rental.Dtos;

namespace Rental.Kafka
{
    public class CustomerKafkaDelete
    {
        public string Action{get;set;} = "DELETE";
        public int CusotmerId{get;set;}
        public int BookId{get;set;} 
        public int RentId{get;set;}
    }
}