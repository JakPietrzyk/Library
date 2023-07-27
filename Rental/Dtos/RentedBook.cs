namespace Rental.Dtos
{
    public class RentedBook
    {
        public DateTimeOffset RentDate{get;set;}
        public string? Title { get; set; }
        public string? Author { get; set; }
        public DateTimeOffset Releasedate { get; set; }
    }
}