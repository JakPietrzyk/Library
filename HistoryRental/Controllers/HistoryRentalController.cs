using HistoryRental.Model;
using HistoryRental.Services;
using HistoryRental.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HistoryRental.Loggers;

namespace HistoryRental.Controllers
{
    [Route("api/history")]
    [ServiceFilter(typeof(LoggerFilterAttribbute))]
    [ApiController]
    public class RentalController: ControllerBase
    {
        private readonly IHistoryRentalService _historyRentalService;
        public RentalController(IHistoryRentalService historyRentalService)
        {
            _historyRentalService = historyRentalService;
        }

        [HttpGet("{id}/{n}")]
        public async Task<ActionResult<IEnumerable<MongoDbRental>>> GetAll([FromRoute]int id, [FromRoute]int n)
        {
            _historyRentalService.AddXRequestId(HttpContext);
            var customers = await _historyRentalService.GetAll(id, n);

            return Ok(customers);
        }
        [HttpPost]
        public async Task<ActionResult> CreateCustomer([FromBody] MongoDbRental dto)
        {
            _historyRentalService.AddXRequestId(HttpContext);
            var id = await _historyRentalService.Create(dto);

            return Created($"/api/rental/{id}", null);
        } 
        
    }
}