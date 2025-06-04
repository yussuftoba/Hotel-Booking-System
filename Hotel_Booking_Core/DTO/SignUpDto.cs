using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;
public class SignUpDto
{
    [Required, Column(TypeName = ("varchar(30)"))]
    public string FirstName { get; set; } = "";

    [Required, Column(TypeName = ("varchar(30)"))]
    public string LastName { get; set; } = "";
    [Required, Column(TypeName = ("varchar(30)")), DataType(DataType.EmailAddress)]
    public string Email { get; set; } = "";

    [Required, Column(TypeName = ("varchar(13)"))]
    public string PhoneNumber { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required, Column(TypeName = ("varchar(30)")), DataType(DataType.Password),Compare("Password")]
    public string ConfirmPassword { get; set; } = "";
}
