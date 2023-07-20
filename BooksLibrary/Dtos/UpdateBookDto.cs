namespace BooksLibrary.Dtos
{
    public class UpdateBookDto
    {

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;
        
        public DateTime Releasedate { get; set; }
    }
}