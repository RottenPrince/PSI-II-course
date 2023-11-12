namespace API.Managers
{
    public interface IRepository<T> where T: class
    {
        Task<List<T>> GetAll();
        IQueryable<T> Query { get; }
        Task<T?> GetById(int id);
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Remove(T entity);
        void Remove(IEnumerable<T> entities);
        Task<int> Save();
    }
}
