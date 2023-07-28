using AutoMapper;
using HistoryRental.Model;
using HistoryRental.Dtos;

namespace HistoryRental.Mappers
{
    public class HistoryRentalMappingProfile: Profile
    {
        public HistoryRentalMappingProfile()
        {
            CreateMap<KafkaRental, MongoDbRental>();
        }

    }
}