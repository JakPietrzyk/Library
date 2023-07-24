using Rental.Model;
using Rental.Services;
using Rental.Dtos;
using Rental.Clients;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Rental.Controllers
{
    [Route("api/rental")]
    [ApiController]
    public class RentalController: ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly BooksClient _client;
        public RentalController(IRentalService rentalService, BooksClient client)
        {
            _rentalService = rentalService;
            _client = client;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CustomerDto>> GetAll([FromQuery]DateTime? from = null, [FromQuery]DateTime? to = null)
        {
            var customers = _rentalService.GetAll(from,to);

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public ActionResult GetRent([FromRoute]int id )
        {
            _rentalService.CheckRent(id);
            return Ok();
        }

        [HttpPost("new")]
        public ActionResult CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var id = _rentalService.Create(dto);

            return Created($"/api/rental/{id}", null);
        } 
        [HttpPost("rent/{id}")]
        public async Task<ActionResult<CustomerDto>> RentBook([FromRoute]int id, [FromBody]CreateCustomerDto dto)
        {
            var book = await _client.GetBook(id);
            var CustomerId = _rentalService.Rent(dto, book);

            return Created($"/api/rental/{CustomerId}" ,null);
        }
        [HttpDelete("rent/{id}")]
        public ActionResult DeleteRent([FromRoute]int id)
        {
            _rentalService.Delete(id);

            return NoContent();
        }
    }
}