using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;
public class PaymentDto
{
    public string PaymentMethod { get; set; }
    public double TotalPrice {  get; set; }
    public string StripeToken { get; set; }
    public int BookingId { get; set; }

}
