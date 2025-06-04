using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;

public class BookingDto
{

    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }

	[DataType(DataType.Date)]

	public DateTime CheckOutDate { get; set; }

    public int PaymentMethod {  get; set; }

}
