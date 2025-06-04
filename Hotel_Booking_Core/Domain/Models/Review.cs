using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;
public class Review
{
    public int Id { get; set; }

    [Column(TypeName = ("decimal(3,2)")), Range(1, 5)]
    public double? Rating {  get; set; }

    public string? Comment {  get; set; }

    public DateTime? ReviewDate { get; set; }

    [ForeignKey("User")]
    public int? UserId { get; set; }
    public User? User { get; set; } 

    [ForeignKey("Hotel")]
    public int? HotelId { get; set; }
    public Hotel? Hotel { get; set; } 
}
