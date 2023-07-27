using Rental.Model;
using Rental.Dtos;
using Rental.Mappers;
using AutoMapper;
using Rental.Exceptions;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using Newtonsoft.Json;
using Rental.Kafka;

namespace Rental.Services
{
    public interface IRentalService
    {
        Task<IEnumerable<CustomerDto>> GetAll(DateTime? from, DateTime? to);
        Task<int> Create(CreateCustomerDto dto);
        bool CheckAvailability(int id);
        Task<int> Rent(CreateCustomerDto dto, BookDto book);
        Task Delete(int id);
        Customer CheckRent(int id);
    }

    public class RentalService: IRentalService
    {
        private readonly IMapper _mapper;
        private readonly RentalContext _context;
        private readonly ILogger<RentalService> _logger;

        public RentalService(RentalContext context,IMapper mapper, ILogger<RentalService> logger)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
            Uri uri = new Uri("http://localhost:9092");    
            string topic = "rentalEvents";
        }

        public Customer CheckRent(int id)
        {
            _logger.LogInformation($"GET Customer with rented book with id: {id} action invoked");
            var result = _context.Customer.FirstOrDefault(c => c.book_id == id);

            if(result is null) throw new NotFoundException("Book is not available");
            _logger.LogInformation($"GET Customer with rented book with id: {id} action invoked");
            return result;
        }
        public async Task<IEnumerable<CustomerDto>> GetAll(DateTime? from, DateTime? to)
        {
            _logger.LogInformation($"GET all Customers {from} {to} action invoked");
            var customers = _context.Customer.AsQueryable();

            if(from.HasValue)
            {
                customers = customers.Where(c => c.Rental_date >= from.Value.Date);
            }
            if(to.HasValue)
            {
                customers = customers.Where(c => c.Rental_date <= to.Value.Date);
            }

            var result = _mapper.Map<List<CustomerDto>>(await customers.ToListAsync());
            _logger.LogInformation($"GET all Customers {from} {to} action executed");
 
            string jsonString = JsonConvert.SerializeObject(_mapper.Map<List<CustomerKafkaGet>>(await customers.ToListAsync()), Formatting.Indented);
            SendMessageToKafka(jsonString);
            return result;
        }

        public async Task<int> Create(CreateCustomerDto dto)
        {
            _logger.LogInformation($"CREATE Customer action invoked");

            var customer = _mapper.Map<Customer>(dto);
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"CREATE Customer action executed");
            return customer.Id;
        }
        public bool CheckAvailability(int id)
        {
            _logger.LogInformation($"GET Customer with rented book with id: {id} action to check availability returns bool invoked");

            Customer? result =_context.Customer.FirstOrDefault(c => c.book_id == id);
            if(result is null) return true;

            _logger.LogInformation($"GET Customer with rented book with id: {id} action to check availability returns bool executed");
            return false;
        }
        public async Task<int> Rent(CreateCustomerDto dto, BookDto book)
        {
            _logger.LogInformation($"CREATE Customer {dto.Surname} with rented book with id: {book.Id} invoked");
            if(!CheckAvailability(book.Id)) throw new NotFoundException("Book is not avaliable");
            
            dto.book_id = book.Id;
            dto.Rental_date = DateTime.Today;

            dto.Author = book.Author;
            dto.Title = book.Title;
            dto.Releasedate = book.Releasedate;

            var id = await Create(dto);
            _logger.LogInformation($"CREATE Customer {dto.Surname} with rented book with id: {book.Id} invoked");
            return id;
        }
        public async Task Delete(int id)
        {
            _logger.LogInformation($"DELETE Customer with id: {id} invoked");
            var customer = _context.Customer.FirstOrDefault(c=>c.Id==id);

            if(customer is null) throw new NotFoundException("Customer not found");

            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"DELETE Customer with id: {id} executed");
        }


        public void SendMessageToKafka(string message)
        {
            var config = new ProducerConfig{
                BootstrapServers = "localhost:9092"
            };
            var kafkaTopic = "rentalEvents";
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    string messageValue = message;

                    producer.Produce(kafkaTopic, new Message<Null, string> { Value = messageValue },
                        deliveryReport =>
                        {
                            if (deliveryReport.Error.Code != ErrorCode.NoError)
                            {
                                Console.WriteLine($"Error: {deliveryReport.Error.Reason}");
                            }
                            else
                            {
                                Console.WriteLine($"Message sent: {deliveryReport.Message.Value}");
                            }
                        });
                }
                catch (ProduceException<Null, string> ex)
                {
                    Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
                }
            }
        }
    }
}