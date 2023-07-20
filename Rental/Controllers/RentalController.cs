using Rental.Model;
using Rental.Services;
using Rental.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Rental.Controllers
{
    [Route("api/rental")]
    [ApiController]
    public class RentalController: ControllerBase
    {
        private readonly IRentalService _rentalService;
        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAll()
        {
            var customers = _rentalService.GetAll();

            return Ok(customers);
        }

        [HttpPost("new")]
        public ActionResult CreateCustomer([FromBody] Customer dto)
        {
            var id = _rentalService.Create(dto);

            return Created($"/api/rental/{id}", null);
        } 
        [HttpPost("rent/{title}")]
        public async Task<ActionResult<CustomerDto>> RentBook([FromRoute]string title, [FromBody]Customer dto)
        {
            HttpClient client = new(){BaseAddress = new Uri("http://localhost:5011")};
            HttpResponseMessage response = await client.GetAsync($"/api/library/book/{title}");

            if(response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                BookDto result = JsonConvert.DeserializeObject<BookDto>(jsonString);

                

                if(result is not null) 
                {
                    dto.book_id = result.Id;
                    dto.Rental_date = DateTime.Today;

                    _rentalService.Create(dto);


                    return Ok(new CustomerDto()
                    {
                        Name = dto.Name,
                        Surname = dto.Surname,
                        Rental_date = dto.Rental_date,
                        Title = result.Title,
                        Author = result.Author,
                        Releasedate = result.Releasedate
                    });
                }
            }

            return BadRequest();
            
        }
    }
}