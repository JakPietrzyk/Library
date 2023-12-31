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
        public DateTime rentDate{get;set;} 
        [BsonIgnoreIfNull]
        public DateTime? returnDate{get;set;} = null;
        public int RentId{get;set;}
        public int cusotmerId{get;set;}
        public int bookId{get;set;} 
    }
}