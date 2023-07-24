using Newtonsoft.Json;
using Rental.Dtos;
using Rental.Exceptions;

namespace Rental.Clients
{
    public class BooksClient
    {
        private readonly HttpClient _client;
        private readonly string _url = "http://localhost:5011/api/library";

        public BooksClient(HttpClient client)
        {
            _client = client;
            
        }

        public async Task<BookDto> GetBook(int id)
        {
            HttpResponseMessage response = await _client.GetAsync($"{_url}/{id}");


            if(response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                BookDto result = JsonConvert.DeserializeObject<BookDto>(jsonString);
                if(result is not null) 
                {
                    return result;
                }
            }
            throw new NotFoundException("Book not found");
        }
    }
}