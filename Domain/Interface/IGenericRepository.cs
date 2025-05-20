using System.Linq.Expressions;

namespace Domain.Interface
{
    public interface IGenericRepository<T, TKey> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(TKey id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteByIdAsync(TKey id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
