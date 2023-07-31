using AutoMapper;
using HistoryRental.Model;
using HistoryRental.Dtos;

namespace HistoryRental.Mappers
{
    public class HistoryRentalMappingProfile: Profile
    {
        public HistoryRentalMappingProfile()
        {
            CreateMap<KafkaRental, MongoDbRental>()
                .ForMember(b => b.rentDate, opt => opt.MapFrom(src => DateTime.Now)); 
            CreateMap<Book,BookDto>();
        }

    }
}