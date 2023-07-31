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

namespace HistoryRental.Services
{
    public interface IHistoryRentalService
    {
        Task<CustomerDto> GetAll(int id, int n);
        Task<string> Create(MongoDbRental dto);
    }

    public class HistoryRentalService: IHistoryRentalService
    {
        private readonly IMongoCollection<MongoDbRental> _rentalCollection;
        private readonly IBooksClient _booksClient;
        private readonly IRentalClient _rentalClient;
        private readonly IMapper _mapper; 
        public HistoryRentalService(IOptions<HistoryRentalDatabaseSettings> historyRentalDatabaseSettings, IBooksClient booksClient, IRentalClient rentalClient, IMapper mapper)
        {
            var mongoConnectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(mongoConnectionString);
            var database = mongoClient.GetDatabase("RentalHistory");
            _rentalCollection = database.GetCollection<MongoDbRental>("RentalHistory");
            _booksClient = booksClient;
            _rentalClient = rentalClient;
            _mapper = mapper;

        }
        public async Task<CustomerDto> GetAll(int id, int n)
        {
            CustomerDto customer;
            try
            {
                customer = await _rentalClient.GetCustomer(id);
            }
            catch
            {
                throw;
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
                    var book = await _booksClient.GetBook(rent.bookId);
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

            return result;

        }
        public async Task<string> Create(MongoDbRental dto)
        {
            await _rentalCollection.InsertOneAsync(dto);
            return dto.Id;
        }
    }
}