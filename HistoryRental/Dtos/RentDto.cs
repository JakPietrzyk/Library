using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoryRental.Dtos;

public partial class RentDto
{
    public DateTimeOffset RentDate{get;set;}
    public BookDto? Book{get;set;}
}
