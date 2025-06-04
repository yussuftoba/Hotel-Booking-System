using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces;
public interface IRepository<T> where T: class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    T Update(T entity);
    void Delete(T entity);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[]? includes = null);
    T FindOneItem(Expression<Func<T, bool>> creiteria, string[]? includes = null);
    int Count(Expression<Func<T, bool>> criteria);
    double RatingSummation(int id);

}
