using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Repositories.Interfaces;
namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            var prop = typeof(T).GetProperty("IsDeleted");
            if (prop != null)
            {
                prop.SetValue(entity, true);
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await GetByIdAsync(id) != null;
        }

        public async Task<IEnumerable<T>> GetAllAsync(string includes = "")
        {
            return await GetAllAsync(0, includes);
        }
        public async Task<IEnumerable<T>> GetAllAsync(int page = 1, string includes = "")
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }

            if (page > 0)
            {
                int pageSize = Constants.Numbers.DefaultPageSize;
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, string includes = "")
        {
            var entity = await _dbSet.FindAsync(id);

            var query = _dbSet.AsQueryable();

            var isDeletedProp = typeof(T).GetProperty("IsDeleted");
            if (entity != null && isDeletedProp != null)
            {
                if ((bool)(isDeletedProp.GetValue(entity) ?? false))
                {
                    return null; // Entity is marked as deleted
                }
            }

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }

            entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);

            return entity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public async Task<IEnumerable<T>> GetDeletedAsync()
        {
            var query = _dbSet.AsQueryable();
            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == true);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support soft deletion.");
            }

            return await query.ToListAsync();
        }

        public void Restore(T entity)
        {
            var isDeleted = typeof(T).GetProperty("IsDeleted");
            var idProp = typeof(T).GetProperty("Id");
            if (isDeleted != null && idProp != null)
            {
                isDeleted.SetValue(entity, false);
                _dbSet.Update(entity);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support restoration.");
            }
        }
    }

}
