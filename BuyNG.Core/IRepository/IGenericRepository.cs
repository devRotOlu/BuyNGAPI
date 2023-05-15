using BuyNG.Core.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using X.PagedList;

namespace BuyNG.Core.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IPagedList<T>> GetAll(RequestParams requestParams, List<string>? includes = null);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? expression = null, List<string>? includes = null);
        Task<IEnumerable<T>> GetAll<T1>(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IIncludableQueryable<T, Object>> includes = null);
        Task<T> Get(Expression<Func<T, bool>> expression, List<string>? includes = null);

        Task Insert(T entity);  

        Task InsertRange(IEnumerable<T> entities);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);

        void Update(T entity);
    }
}
