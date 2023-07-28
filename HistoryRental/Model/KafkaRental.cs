using System.Collections.Generic;
using HistoryRental.Dtos;

namespace HistoryRental.Model
{
    public class KafkaRental
    {
        public string? Action{get;set;}
        public List<CustomerDto>? customerDto {get;set;} = new List<CustomerDto>();
    }
}
