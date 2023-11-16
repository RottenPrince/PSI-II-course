using BrainBoxAPI.Data;
using BrainBoxAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BrainBoxAPI.Managers
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

        public int Save()
        {
            try
            {
                return _context.SaveChanges();
            } catch (DbUpdateException ex)
            {
                var message = ex.InnerException.Message;
                var errorCode = Regex.Match(message, "^.*Error (\\d+):.*$");
                if (!errorCode.Success || errorCode.Groups.Count < 2) { throw ex; }
                var errorCodeInt = int.Parse(errorCode.Groups[1].Value);
                if (errorCodeInt == 19) { throw new DbConstraintFailedException(ex); }
                else { throw ex; }
            }
        }
    }
}
