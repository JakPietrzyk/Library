namespace Rental.Dtos
{
    public class CustomerDto
    {
        public string Name{get;set;}
        public string Surname{get;set;}
        public DateTime Rental_date{get;set;}
        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public DateTime Releasedate { get; set;}
    }
}