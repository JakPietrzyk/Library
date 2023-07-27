using Rental.Dtos;

namespace Rental.Kafka
{
    public class CustomerKafkaDelete
    {
        public string Action{get;set;} = "Delete";
        public int id{get;set;}
        public string? Name{get;set;}
        public string? Surname{get;set;}
        public DateTimeOffset Rental_date{get;set;}
        public List<RentedBook>? rentedBooks{get;set;}
    }
}