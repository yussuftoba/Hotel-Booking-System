using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;
public class DashboardContentDto
{
    public int TotalHotels {  get; set; }
    public int ActiveRooms {  get; set; }
    public double Revenue { get; set; } = 0;
    public IEnumerable<Booking> BookingsInfo { get; set; }
}
