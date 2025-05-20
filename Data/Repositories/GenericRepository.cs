using Data.Context;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.IncludeAll(_context).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault();

            if (primaryKey == null)
            {
                return await _dbSet.IncludeAll(_context).FirstOrDefaultAsync();
            }

            return await _dbSet.IncludeAll(_context)
                .FirstOrDefaultAsync(e => EF.Property<TKey>(e, primaryKey.Name)!.Equals(id));
        }


        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
