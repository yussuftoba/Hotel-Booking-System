using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DTO;

public class HotelDto
{
    [Required, Column(TypeName = ("varchar(30)"))]
    public string Name { get; set; }

    [Required, Column(TypeName = ("varchar(100)"))]
    public string Address { get; set; }

    [Required, Column(TypeName = ("varchar(30)"))]
    public string City { get; set; }

    [Required, Column(TypeName = ("varchar(30)"))]
    public string Country { get; set; }

    [Required, Column(TypeName = ("varchar(13)"))]
    public string PhoneNumber { get; set; }

    [Required, Column(TypeName = ("varchar(30)")), DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    public string Description { get; set; }

    public IFormFile ImageFile { get; set; }

}

