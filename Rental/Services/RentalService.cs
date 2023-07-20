using Rental.Model;
using Rental.Dtos;
using Rental.Mapper;
using AutoMapper;

namespace Rental.Services
{
    public interface IRentalService
    {
        IEnumerable<Customer> GetAll();
        int Create(Customer dto);
    }

    public class RentalService: IRentalService
    {
        private readonly IMapper _mapper;
        private readonly RentalContext _context;

        public RentalService(RentalContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        public IEnumerable<Customer> GetAll()
        {
            var customer = _context.Customer.ToList();

            return customer;
        }

        public int Create(Customer dto)
        {
            _context.Customer.Add(dto);
            _context.SaveChanges();

            return dto.Id;
        }
    }
}