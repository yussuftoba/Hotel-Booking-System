using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;
public class Booking
{
    public int Id { get; set; }

    public DateTime BookingDate {  get; set; }

    [Required, Column(TypeName = ("date"))]
    public DateTime CheckInDate { get; set; }

    [Required, Column(TypeName = ("date"))]
    public DateTime CheckOutDate { get; set; }

    [Required, Column(TypeName = ("decimal(6,1)")), Range(1, 99999)]
    public double TotalPrice {  get; set; }

    [Required, Column(TypeName = ("varchar(30)"))]
    public string PaymentStatus { get; set; }

    [Required, Column(TypeName = ("varchar(30)"))]
    public string BookingStatus { get; set; }

    [ForeignKey("PaymentMethod")]
    public int? PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }

    [ForeignKey("User")]
    public int? UserId {  get; set; }
    public User? User { get; set; }

    [ForeignKey("Room")]
    public int? RoomId { get; set; }
    public Room? Room { get; set; }


}
