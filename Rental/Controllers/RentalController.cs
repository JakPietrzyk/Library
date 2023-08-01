using Rental.Model;
using Rental.Services;
using Rental.Dtos;
using Rental.Clients;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rental.Middleware;

namespace Rental.Controllers
{
    [ApiController]
    [Route("api/rental")]
    [ServiceFilter(typeof(LoggerFilterAttribbute))]
    public class RentalController: ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly IBooksClient _client;
        public RentalController(IRentalService rentalService, IBooksClient client)
        {
            _rentalService = rentalService;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll([FromQuery]DateTime? from = null, [FromQuery]DateTime? to = null)
        {
            var customers = await _rentalService.GetAll(from,to);

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public ActionResult GetRent([FromRoute]int id )
        {
            _rentalService.CheckRent(id);
            return Ok();
        }
        [HttpGet("customer/{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer([FromRoute]int id)
        {
            var customer = await _rentalService.GetCustomer(id);
            return Ok(customer);
        }
        [HttpPost("rent/{id}")]
        public async Task<ActionResult<CustomerDto>> RentBook([FromRoute]int id, [FromBody]CreateCustomerDto dto)
        {
            var book = await _client.GetBook(id);
            var CustomerId = await _rentalService.Rent(dto, book);

            return Created($"/api/rental/{CustomerId}" ,null);
        }
        [HttpPost("{customerId}/book/{bookId}")]
        public async Task<ActionResult> AddRentToCustomer([FromRoute]int customerId, [FromRoute]int bookId)
        {
            var book = await _client.GetBook(bookId);
            await _rentalService.Update(customerId, book);

            return Created($"/api/rental/{customerId}" ,null);
        }
        [HttpDelete("rent/{id}")]
        public async Task<ActionResult> DeleteRent([FromRoute]int id)
        {
            await _rentalService.DeleteRent(id);

            return NoContent();
        }
        [HttpDelete("customer/{id}")]
        public async Task<ActionResult> DeleteCustomer([FromRoute]int id)
        {
            await _rentalService.Delete(id);

            return NoContent();
        }
    }
}