using Newtonsoft.Json;
using HistoryRental.Dtos;
using HistoryRental.Exceptions;

namespace HistoryRental.Clients
{
    public interface IRentalClient
    {
        Task<CustomerDto> GetCustomer(int id);
    }
    public class RentalClient: IRentalClient
    {


        private readonly HttpClient _client;
        private readonly string _url = "http://localhost:5024/api/rental";

        public RentalClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<CustomerDto> GetCustomer(int id)
        {
            HttpResponseMessage response = new();
            try
            {   
                string requestId = Guid.NewGuid().ToString();
                _client.DefaultRequestHeaders.Add("X-Request-ID", requestId);
                
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