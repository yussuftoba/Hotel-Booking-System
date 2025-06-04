using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces;
public interface IUnitOfWork : IDisposable
{
    public IRepository<Booking> Bookings { get; }
    public IRepository<Hotel> Hotels { get; }
    public IRepository<Review> Reviews { get; }
    public IRepository<Room> Rooms { get; }
    public IRepository<User> Users { get; }
    public IRepository<PaymentMethod> PaymentMethods { get; }
    public IRepository<ResetPasswords> ResetPasswords { get; }


    int Save();
}
