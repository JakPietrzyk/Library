using HistoryRental.Model;
using HistoryRental.Dtos;
using HistoryRental.Mappers;
using HistoryRental.Clients;
using AutoMapper;
using HistoryRental.Exceptions;
using Confluent.Kafka;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using HistoryRental.Settings;

namespace HistoryRental.Services
{
    public interface IHistoryRentalService
    {
        Task<CustomerDto> GetAll(int id, int n);
        Task<string> Create(MongoDbRental dto);
        void AddXRequestId(HttpContext context);
    }

    public class HistoryRentalService: IHistoryRentalService
    {
        private readonly IMongoCollection<MongoDbRental> _rentalCollection;
        private readonly IBooksClient _booksClient;
        private readonly IRentalClient _rentalClient;
        private readonly IMapper _mapper; 
        private readonly ILogger<HistoryRentalService> _logger;
        // private readonly IMongoClient _mongoClient;
        public HistoryRentalService(IOptions<HistoryRentalDatabaseSettings> historyRentalDatabaseSettings, IBooksClient booksClient, IRentalClient rentalClient, IMapper mapper, ILogger<HistoryRentalService> logger)
        {
            // _mongoClient = mongoClient;
            var mongoClient = new MongoClient(
            historyRentalDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                historyRentalDatabaseSettings.Value.DatabaseName);

            _rentalCollection = mongoDatabase.GetCollection<MongoDbRental>(
                historyRentalDatabaseSettings.Value.RentalCollectionName);

            _booksClient = booksClient;
            _rentalClient = rentalClient;
            _mapper = mapper;
            _logger = logger;
        }
        public void AddXRequestId(HttpContext context)
        {
            var listToUpdate = (List<string>?)context.Items["X-Request-ID"];
            string requestId = listToUpdate.Last();

            _rentalClient.SetXRequestId(requestId);
            _booksClient.SetXRequestId(requestId);
        }
        public async Task<CustomerDto> GetAll(int id, int n)
        {
            _logger.LogDebug(($"GetAll Customer with id: {id} invoked"));
            CustomerDto customer;
            string requestId = Guid.NewGuid().ToString();
            
            
            try
            {
                
                customer = await _rentalClient.GetCustomer(id);
            }
            catch
            {
                throw new NotFoundException("Customer not found");
            }


            var filter = Builders<MongoDbRental>.Filter.Eq(r => r.cusotmerId, id) &
                         Builders<MongoDbRental>.Filter.Exists(r => r.returnDate);

            var sort = Builders<MongoDbRental>.Sort.Descending(r => r.returnDate);

            var resultFromDatabase = await _rentalCollection.Find(filter)
                                               .Sort(sort)
                                               .Limit(n)
                                               .ToListAsync();

             var listOfRents = new List<RentDto>();
                foreach(var rent in resultFromDatabase)
                {
                    Book book;
                    try
                    {
                        book = await _booksClient.GetBook(rent.bookId);
                    }
                    catch
                    {
                        throw new NotFoundException("Book not found");
                    }
                    listOfRents.Add(new RentDto{
                                ReturnDate = rent.returnDate,
                                RentDate = rent.rentDate,
                                Book = _mapper.Map<BookDto>(book)
                            });
                }
                var result = new CustomerDto{
                        Name = customer.Name,
                        Surname = customer.Surname,
                        Rents = listOfRents
                    };
            _logger.LogDebug(($"GetAll Customer with id: {id} executed"));
            return result;

        }
        public async Task<string> Create(MongoDbRental dto)
        {
            _logger.LogDebug(($"Create Rental with rentDate: {dto.rentDate} invoked"));
            await _rentalCollection.InsertOneAsync(dto);
            _logger.LogDebug(($"Create Rental with rentDate: {dto.rentDate} executed"));
            return dto.Id;
        }
    }
}