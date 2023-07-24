using Rental.Model;
using Rental.Dtos;
using Rental.Mapper;
using AutoMapper;
using Rental.Exceptions;

namespace Rental.Services
{
    public interface IRentalService
    {
        IEnumerable<CustomerDto> GetAll(DateTime? from, DateTime? to);
        int Create(CreateCustomerDto dto);
        bool CheckAvailability(int id);
        int Rent(CreateCustomerDto dto, BookDto book);
        void Delete(int id);
        Customer CheckRent(int id);
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

        public Customer CheckRent(int id)
        {
            var result = _context.Customer.FirstOrDefault(c => c.book_id == id);

            if(result is null) throw new NotFoundException("Book is not available");

            return result;
        }
        public IEnumerable<CustomerDto> GetAll(DateTime? from, DateTime? to)
        {
            var customers = _context.Customer.AsQueryable();

            if(from.HasValue)
            {
                customers = customers.Where(c => c.Rental_date >= from.Value.Date);
            }
            if(to.HasValue)
            {
                customers = customers.Where(c => c.Rental_date <= to.Value.Date);
            }

            var result = _mapper.Map<List<CustomerDto>>(customers);
            return result;
        }

        public int Create(CreateCustomerDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            _context.Customer.Add(customer);
            _context.SaveChanges();

            return customer.Id;
        }
        public bool CheckAvailability(int id)
        {
            Customer result =_context.Customer.FirstOrDefault(c => c.book_id == id);
            if(result is null) return true;

            return false;
        }
        public int Rent(CreateCustomerDto dto, BookDto book)
        {
            if(CheckAvailability(book.Id))
            {

                dto.book_id = book.Id;
                dto.Rental_date = DateTime.Today;

                dto.Author = book.Author;
                dto.Title = book.Title;
                dto.Releasedate = book.Releasedate;

                var id = Create(dto);

                return id;
            }
            throw new NotFoundException("Book is not avaliable");
        }
        public void Delete(int id)
        {
            var customer = _context.Customer.FirstOrDefault(c=>c.Id==id);

            if(customer is null) throw new NotFoundException("Customer not found");

            _context.Customer.Remove(customer);
            _context.SaveChanges();
        }
    }
}