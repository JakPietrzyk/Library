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
                        var reuqestId = Guid.NewGuid();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("X-Request-Id", reuqestId.ToString());
            
        }

        public async Task<CustomerDto> GetCustomer(int id)
        {
            HttpResponseMessage response = new();
            try
            {   
                response = await _client.GetAsync($"{_url}/customer/{id}");
                if (response.Headers.TryGetValues("X-Request-ID", out IEnumerable<string> requestIdValues))
                {
                    // response.Context.Items["X-Request-ID"]
                    string requestId = requestIdValues.FirstOrDefault();
                    // Now you have the X-Request-ID from the response header
                    // You can use it as needed
                    // For example, you can log it or process it further
                }
                
                // string requestId = response.Headers.Contains("X-Request-ID")
                // ? response.Headers.FirstOrDefault(x => x."X-Request-ID"].ToString()
                // : Guid.NewGuid().ToString();
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