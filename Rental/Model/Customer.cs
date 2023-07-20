namespace Rental.Model
{
    public class Customer
    {
        public int Id{get;set;}
        public string Name{get;set;}
        public string Surname{get;set;}
        public DateTime Rental_date{get;set;}
        public int book_id{get;set;}
    }
}