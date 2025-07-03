using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;
public class RoomDto
{
	public int RoomNumber {  get; set; }

    [Required, Column(TypeName = ("varchar(30)"))]
    public string Floor { get; set; } 

    [Required, Column(TypeName = ("varchar(30)"))]
    public string Type { get; set; }

    [Required, Column(TypeName = ("decimal(6,1)")), Range(1,99999)]
    public double BasePrice {  get; set; }

    [Required]
    public string SpecialFeatures { get; set; } 

    public IFormFile ImageUrl { get; set; }
}
