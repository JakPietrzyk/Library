using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoryRental.Dtos;

public partial class RentDto
{
    public DateTime RentDate{get;set;}
    public DateTime? ReturnDate{get;set;} = null; 
    public BookDto? Book{get;set;}
}
