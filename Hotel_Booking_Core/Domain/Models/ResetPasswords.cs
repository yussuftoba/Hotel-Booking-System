using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class ResetPasswords
{
    public int Id {  get; set; }
    [Required]
    public string Token {  get; set; }
    [Required, Column(TypeName =("varchar(40)")), DataType(DataType.EmailAddress)]
    public string Email {  get; set; }

    public DateTime CreatedAt { get; set; }

}
