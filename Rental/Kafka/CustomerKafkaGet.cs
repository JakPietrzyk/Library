using Rental.Dtos;

namespace Rental.Kafka
{
    public class CustomerKafkaGet
    {
        public string Action{get;set;} = "GET";
        public int id{get;set;}
        public string? Name{get;set;}
        public string? Surname{get;set;}
        public DateTimeOffset Rental_date{get;set;}
        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public DateTimeOffset Releasedate { get; set;}
        public List<RentedBook>? rentedBooks{get;set;}
    }
}