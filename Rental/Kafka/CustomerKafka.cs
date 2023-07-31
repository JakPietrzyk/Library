using Rental.Dtos;

namespace Rental.Kafka
{
    public class CustomerKafka
    {
        public DateTime RentDate{get;set;}
        public DateTime? ReturnDate{get;set;} = null;
        public int CusotmerId{get;set;}
        public int BookId{get;set;} 
        public int RentId{get;set;}
    }
}