using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;
public class PaymentMethod
{
    public int Id { get; set; }

    [Required, Column(TypeName =("varchar(30)"))]
    public string MethodName { get; set; } 
}
