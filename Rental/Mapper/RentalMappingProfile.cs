using AutoMapper;
using Rental.Model;
using Rental.Dtos;
using Rental.Kafka;

namespace Rental.Mappers
{
    public class RentalMappingProfile: Profile
    {
        public RentalMappingProfile()
        {
            CreateMap<Customer,CustomerDto>();

            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<BookDto, RentedBook>()
                .ForMember(b => b.RentDate, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));
            CreateMap<CustomerDto, CustomerKafkaGet>();
            CreateMap<Customer, CustomerKafkaGet>();
        }

    }
}