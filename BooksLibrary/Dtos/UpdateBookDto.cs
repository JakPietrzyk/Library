namespace BooksLibrary.Dtos
{
    public class UpdateBookDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public DateTimeOffset   Releasedate { get; set; }
    }
}