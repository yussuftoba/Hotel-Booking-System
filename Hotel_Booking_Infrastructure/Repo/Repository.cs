﻿using Context;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repo;
public class Repository<T> : IRepository<T> where T : class
{
    private HotelBookingDbContext _context;
    public Repository(HotelBookingDbContext context)
    {
        _context = context;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.AddAsync(entity);
        return entity;
    }

    public void Delete(T entity)
    {
        _context.Remove(entity);
    }

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[]? includes = null)
    {
        IQueryable<T> queries = _context.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                queries = queries.Include(include);
            }
        }
        return await queries.Where(criteria).ToListAsync();
    }

    public T FindOneItem(Expression<Func<T, bool>> creiteria, string[]? includes = null)
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return query.SingleOrDefault(creiteria);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public T Update(T entity)
    {
        _context.Update(entity);
        return entity;
    }

    public IEnumerable<T> Pagenation(Expression<Func<T, bool>> criteria, int page, int pageSize, string[]? includes = null)
    {
        IQueryable<T> queries = _context.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                queries = queries.Include(include);
            }
        }


        return queries.Where(criteria).Skip<T>((page - 1) * pageSize).Take<T>(pageSize).ToList();

    }

    public int Count(Expression<Func<T, bool>> criteria)
    {
        return _context.Set<T>().Count(criteria);
    }

    public double RatingSummation(int id)
    {
        var sumOfRating = _context.Reviews.Where(r=>r.HotelId==id).Sum(r => r.Rating?? 0);

        return sumOfRating;
    }


}
