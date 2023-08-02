using Newtonsoft.Json;
using Rental.Dtos;
using Rental.Exceptions;

namespace Rental.Clients
{
    public interface IBooksClient
    {
        Task<Book> GetBook(int id);
        public void SetXRequestId(string requestId);
    }
    public class BooksClient: IBooksClient
    {
        private readonly HttpClient _client;
        private readonly string _url = "http://localhost:5011/api/library";

        public BooksClient(HttpClient client)
        {
            _client = client;
            
        }
        public void SetXRequestId(string requestId)
        {
            if (_client.DefaultRequestHeaders.Contains("X-Request-ID")) _client.DefaultRequestHeaders.Remove("X-Request-ID");
            _client.DefaultRequestHeaders.Add("X-Request-ID", requestId);
        }
        public async Task<Book> GetBook(int id)
        {
            HttpResponseMessage response = new();
            try
            {
                response = await _client.GetAsync($"{_url}/{id}");
            }
            catch
            {
                throw new BadHttpRequestException("Can not connect to Library service");
            }


            if(response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                Book? result = JsonConvert.DeserializeObject<Book>(jsonString);
                if(result is not null) 
                {
                    return result;
                }
            }
            throw new NotFoundException("Book not found");
        }
    }
}