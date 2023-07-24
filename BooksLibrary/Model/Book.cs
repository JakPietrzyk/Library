using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksLibrary.Model;

public partial class Book
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Author { get; set; } = null!;
    [Required]
    public DateTimeOffset  Releasedate { get; set; }
}
