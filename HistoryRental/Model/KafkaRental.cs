using System.Collections.Generic;
using HistoryRental.Dtos;

namespace HistoryRental.Model
{
    public class KafkaRental
    {
        public DateTime RentDate{get;set;}
        public DateTime? ReturnDate{get;set;} = null;
        public int CusotmerId{get;set;}
        public int BookId{get;set;} 
        public int RentId{get;set;}
    }
}
