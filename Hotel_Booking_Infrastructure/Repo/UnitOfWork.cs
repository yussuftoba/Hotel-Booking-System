using Context;
using Interfaces;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo;

public class UnitOfWork : IUnitOfWork
{
    private readonly HotelBookingDbContext _context;

    public UnitOfWork(HotelBookingDbContext context)
    {
        _context = context;
        Bookings = new Repository<Booking>(context);
        Hotels=new Repository<Hotel>(context);
        Reviews=new Repository<Review>(context);
        Rooms=new Repository<Room>(context);
        Users=new Repository<User>(context);
        PaymentMethods=new Repository<PaymentMethod>(context);
        ResetPasswords=new Repository<ResetPasswords>(context);
    }
    public IRepository<Booking> Bookings { get; }

    public IRepository<Hotel> Hotels { get; }

    public IRepository<Review> Reviews { get; }

    public IRepository<Room> Rooms { get; }

    public IRepository<User> Users { get; }

    public IRepository<PaymentMethod> PaymentMethods { get; }

    public IRepository<ResetPasswords> ResetPasswords { get; }


    public void Dispose()
    {
        _context.Dispose();
    }

    public int Save()
    {
        return _context.SaveChanges();
    }
}
