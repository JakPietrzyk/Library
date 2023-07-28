using System.Collections.Generic;
using HistoryRental.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace HistoryRental.Model
{
    public enum RentStatus
    {
        Returned = 0,
        Rented = 1
    }
    public class MongoDbRental
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id {get;set;}
        public RentStatus rentStatus {get;set;} = RentStatus.Rented;
        public List<CustomerDto>? customerDto {get;set;} = new List<CustomerDto>();
        // public int cusotmerId{get;set;}
        // public int bookId{get;set;} 
    }
}