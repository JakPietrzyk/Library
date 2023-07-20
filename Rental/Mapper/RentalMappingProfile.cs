using AutoMapper;
using Rental.Model;
using Rental.Dtos;

namespace Rental.Mapper
{
    public class RentalMappingProfile: Profile
    {
        public RentalMappingProfile()
        {
            CreateMap<Customer,CustomerDto>();
        }

    }
}