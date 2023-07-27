using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rental.Model;

public partial class Rent
{
    
    [Required]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "DATE")]
    public DateTimeOffset RentDate{get;set;}
    public int bookId{get;set;}
    public int CustomerId{get;set;}
    public virtual Customer Customer{get;set;} = null!;
}
