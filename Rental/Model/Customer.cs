namespace Rental.Model
{
    public class Customer
    {
        public int Id{get;set;}
        public string Name{get;set;}
        public string Surname{get;set;}
        public DateTimeOffset Rental_date{get;set;}
        public int book_id{get;set;}
        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public DateTimeOffset Releasedate { get; set;}
    }
}