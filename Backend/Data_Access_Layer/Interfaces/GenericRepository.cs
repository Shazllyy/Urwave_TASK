using Data_Access_Layer.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbcontext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbcontext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Get entity by Id (GUID)
        public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            // Apply includes dynamically if any are passed
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id);
        }
        // Get all entities
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Find entities by predicate (filter)
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // Add a single entity
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Add multiple entities
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        // Update an entity
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        // Remove a single entity
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        // Remove multiple entities
        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // Asynchronous update
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        // Asynchronous delete
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        // Server-side filtering, sorting, and pagination
        public async Task<(IEnumerable<T>, int)> GetPaginatedAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int pageIndex = 0,
            int pageSize = 10)
        {
            IQueryable<T> query = _dbSet;

            // Apply filtering if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply sorting if provided
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Get the total count of items
            int totalItems = await query.CountAsync();

            // Apply pagination (skip and take)
            var items = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

            return (items, totalItems);
        }
    }
}
