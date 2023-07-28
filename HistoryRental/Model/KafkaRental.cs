using System.Collections.Generic;
using HistoryRental.Dtos;

namespace HistoryRental.Model
{
    public class KafkaRental
    {
        public string? Action{get;set;}
        public int CusotmerId{get;set;}
        public int BookId{get;set;} 
        public int RentId{get;set;}
    }
}
