using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context;

public class HotelBookingDbContext:DbContext
{
    public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options):base(options)
    {
    }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Hotel>Hotels { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<ResetPasswords> ResetPasswords { get; set; }
}
