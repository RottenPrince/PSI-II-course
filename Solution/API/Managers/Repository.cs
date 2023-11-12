using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Managers
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public abstract IQueryable<T> Query { get; }
        public abstract Task<T?> GetById(int id);

        public Task<List<T>> GetAll()
        {
            return Query.ToListAsync();
        }

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public void Add(IEnumerable<T> entities)
        {
            _context.Add(entities);
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }

        public void Remove(IEnumerable<T> entities)
        {
            _context.Remove(entities);
        }

        public Task<int> Save()
        {
            return _context.SaveChangesAsync();
        }
    }
}
