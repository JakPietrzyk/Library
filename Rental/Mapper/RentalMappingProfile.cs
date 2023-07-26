using AutoMapper;
using Rental.Model;
using Rental.Dtos;

namespace Rental.Mappers
{
    public class RentalMappingProfile: Profile
    {
        public RentalMappingProfile()
        {
            CreateMap<Customer,CustomerDto>();

            CreateMap<CreateCustomerDto, Customer>();
        }

    }
}