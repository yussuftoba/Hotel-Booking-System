using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;

public class ResetPasswordDto
{
    public string Token { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
