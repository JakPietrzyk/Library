using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Rental.Model
{
    public class Customer
    {
        public int Id{get;set;}
        public string? Name{get;set;}
        public string? Surname{get;set;}
        [Column(TypeName = "DATE")]
        public DateTimeOffset Rental_date{get;set;}
        public virtual ICollection<Rent> Rents{get;set;} = new List<Rent>();
    }
}