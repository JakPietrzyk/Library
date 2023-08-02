using Rental.Model;
using Rental.Dtos;
using Rental.Mappers;
using AutoMapper;
using Rental.Exceptions;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using Newtonsoft.Json;
using Rental.Kafka;
using Rental.Clients;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Rental.Services
{
    public interface IRentalService
    {
        Task<IEnumerable<CustomerDto>> GetAll(DateTime? from, DateTime? to);
        Task<CustomerDto> GetCustomer(int id);
        Task<int> Create(Customer dto);
        bool CheckAvailability(int id);
        Task<int> Rent(CreateCustomerDto dto, Book book);
        Task Delete(int id);
        Customer CheckRent(int id);
        Task Update(int customerId, Book book);
        Task DeleteRent(int id);
    }
    public class IgnoreReturnedDateContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyName == "ReturnDate")
            {
                property.ShouldSerialize = instance => false;
            }

            return property;
        }
    }
    public class RentalService: IRentalService
    {
        private readonly IMapper _mapper;
        private readonly RentalContext _context;
        private readonly ILogger<RentalService> _logger;
        private readonly IBooksClient _client;

        public RentalService(RentalContext context,IMapper mapper, ILogger<RentalService> logger,IBooksClient client)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _client = client;
            Uri uri = new Uri("http://localhost:9092");    
            string topic = "rentalEvents";
        }

        public Customer CheckRent(int id)
        {
            _logger.LogInformation($"GET Customer with rented book with id: {id} action invoked");
            var result = _context.Customer.FirstOrDefault(c => c.Rents.Any(b => b.bookId == id));
            
            if(result is null) throw new NotFoundException("Book is not available");
            _logger.LogInformation($"GET Customer with rented book with id: {id} action invoked");
            return result;
        }
        public async Task<IEnumerable<CustomerDto>> GetAll(DateTime? from, DateTime? to)
        {
            _logger.LogInformation($"GET all Customers {from} {to} action invoked");
            var customersQuery = _context.Customer.AsQueryable();

            if(from.HasValue)
            {
                customersQuery = customersQuery.Where(c => c.Rents.Any(d => d.RentDate >= from.Value.Date));
            }
            if(to.HasValue)
            {
                customersQuery = customersQuery.Where(c => c.Rents.Any(d => d.RentDate <= to.Value.Date));
            }
            var customers = await customersQuery.Include(c => c.Rents).ToListAsync();
            

            var listOfCustomers = _mapper.Map<List<Customer>>(customers);
            var result = new List<CustomerDto>();
            foreach(var customer in listOfCustomers)
            {
                var listOfRents = new List<RentDto>();
                foreach(var rent in customer.Rents)
                {
                    var book = await _client.GetBook(rent.bookId);
                    listOfRents.Add(new RentDto{
                                RentDate = rent.RentDate,
                                Book = _mapper.Map<BookDto>(book)
                            });
                }
                result.Add(new CustomerDto{
                        Name = customer.Name,
                        Surname = customer.Surname,
                        Rents = listOfRents
                    });
            }
            
            _logger.LogInformation($"GET all Customers {from} {to} action executed");

            return result;
        }
        public async Task<CustomerDto> GetCustomer(int id)
        {
            _logger.LogInformation($"GET Customer with id {id} action invoked");
            var customer = await _context.Customer.FirstOrDefaultAsync(c => c.Id == id);
            if(customer is null) throw new NotFoundException("Customer not found");
            
            _logger.LogInformation($"GET Customer with id {id} action executed");
            return _mapper.Map<CustomerDto>(customer);
        }
        public async Task<int> Create(Customer customer)
        {
            _logger.LogInformation($"CREATE Customer action invoked");

            // var customer = _mapper.Map<Customer>(dto);
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"CREATE Customer action executed");
            return customer.Id;
        }
        public bool CheckAvailability(int id)
        {
            _logger.LogInformation($"GET Customer with rented book with id: {id} action to check availability returns bool invoked");

            Customer? result =_context.Customer.FirstOrDefault(c => c.Rents.Any(b => b.bookId == id));
            if(result is null) return true;

            _logger.LogInformation($"GET Customer with rented book with id: {id} action to check availability returns bool executed");
            return false;
        }
        public async Task<int> Rent(CreateCustomerDto dto, Book book)
        {
            _logger.LogInformation($"CREATE Customer {dto.Surname} with rented book with id: {book.Id} invoked");
            if(!CheckAvailability(book.Id)) throw new NotFoundException("Book is not avaliable");
            // var rentedBook = _mapper.Map<Boo>(book);

            var rent = new Rent{
                RentDate = DateTimeOffset.UtcNow,
                bookId = book.Id
            };
            var customerToAdd = _mapper.Map<Customer>(dto);
            customerToAdd.Rents.Add(rent);
            
            

            var id = await Create(customerToAdd);
            _logger.LogInformation($"CREATE Customer {dto.Surname} with rented book with id: {book.Id} invoked");
            var addToKafka = _mapper.Map<CustomerKafka>(rent);
            addToKafka.CusotmerId = rent.CustomerId;
            addToKafka.RentId = rent.Id;
            // addToKafka.ReturnDate = DateTime.Now;
            
            var settingsJson = new JsonSerializerSettings
            {
                ContractResolver = new IgnoreReturnedDateContractResolver(),
                Formatting = Formatting.Indented
            };

            string jsonString = JsonConvert.SerializeObject(addToKafka, settingsJson);
            SendMessageToKafka(jsonString);
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
        public async Task DeleteRent(int id)
        {
            _logger.LogInformation($"DELETE Customer with id: {id} invoked");
            // var customer = _context.Customer.FirstOrDefault(c => c.Rents.Any(r => r.Id == id));
            var rent = _context.Rent.FirstOrDefault(r => r.Id == id);
            if(rent is null) throw new NotFoundException("Rent not found");
            
            _context.Rent.Remove(rent);
            await _context.SaveChangesAsync();

            var addToKafka = _mapper.Map<CustomerKafka>(rent);
            addToKafka.CusotmerId = rent.CustomerId;
            addToKafka.RentId = rent.Id;
            addToKafka.ReturnDate = DateTime.Now;
            string jsonString = JsonConvert.SerializeObject(addToKafka, Formatting.Indented);
            SendMessageToKafka(jsonString);

            _logger.LogInformation($"DELETE Customer with id: {id} executed");
        }
        public async Task Update(int customerId, Book book)
        {
            _logger.LogInformation(($"UPDATE Customer with id: {customerId} invoked"));
            var customerToUpdate = await _context.Customer.Include(c => c.Rents).FirstOrDefaultAsync(c => c.Id == customerId);
            if(customerToUpdate is null) throw new NotFoundException("Customer not found");
            if(!CheckAvailability(book.Id)) throw new BadHttpRequestException("Book is not avaliable");

            var rentToAdd = new Rent{
                RentDate = DateTimeOffset.UtcNow,
                bookId = book.Id,
                CustomerId = customerId,
                Customer = customerToUpdate
            };
            customerToUpdate.Rents.Add(rentToAdd);

            await _context.SaveChangesAsync();

            var addToKafka = _mapper.Map<CustomerKafka>(rentToAdd);
            addToKafka.CusotmerId = rentToAdd.CustomerId;
            addToKafka.RentId = rentToAdd.Id;
            string jsonString = JsonConvert.SerializeObject(addToKafka, Formatting.Indented);
            SendMessageToKafka(jsonString);

            _logger.LogInformation(($"UPDATE Customer with id: {customerId} invoked"));
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
                    producer.Flush(new TimeSpan(0,0,15));
                }
                catch (ProduceException<Null, string> ex)
                {
                    Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
                }
            }
        }
    }
}