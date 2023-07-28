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
            // CreateMap<BookDto, RentedBook>()
            //     .ForMember(b => b.RentDate, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));
            // CreateMap<BookDto, Book>()
            //     .ForMember(b => b.RentDate, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));
            // CreateMap<Book, RentedBook>()
            //     .ForMember(b => b.RentDate, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));    
            CreateMap<Book, BookDto>()
                .ForMember(b=>b.Releasedate, opt => opt.MapFrom(src => src.Releasedate.DateTime.Date));
            // CreateMap<RentedBook, Book>();
            CreateMap<CustomerDto, CustomerKafkaGet>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<Customer, CustomerDto>();
            CreateMap<Rent, RentDto>();
            CreateMap<RentDto, Rent>();
            CreateMap<Customer, CustomerKafkaGet>();
            CreateMap<Rent,CustomerKafkaGet>();
                // .ForMember(b => b.CusotmerId, opt => opt.MapFrom(src => CustomerId));
        }

    }
}