using Newtonsoft.Json;
using HistoryRental.Dtos;
using HistoryRental.Exceptions;

namespace HistoryRental.Clients
{
    public interface IRentalClient
    {
        Task<CustomerDto> GetCustomer(int id);
        public void SetXRequestId(string requestId);
    }
    public class RentalClient: IRentalClient
    {
        private readonly HttpClient _client;
        private readonly string _url = "http://localhost:5024/api/rental";

        public RentalClient(HttpClient client)
        {
            _client = client;
        }
        public void SetXRequestId(string requestId)
        {
            if (_client.DefaultRequestHeaders.Contains("X-Request-ID")) _client.DefaultRequestHeaders.Remove("X-Request-ID");
            _client.DefaultRequestHeaders.Add("X-Request-ID", requestId);
        }
        public async Task<CustomerDto> GetCustomer(int id)
        {
            HttpResponseMessage response = new();
            try
            {   
                response = await _client.GetAsync($"{_url}/customer/{id}");
            }
            catch
            {
                throw new BadHttpRequestException("Can not connect to Rental service");
            }


            if(response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                CustomerDto? result = JsonConvert.DeserializeObject<CustomerDto>(jsonString);
                if(result is not null) 
                {
                    return result;
                }
            }
            throw new NotFoundException("Customer not found");
        }
    }
}