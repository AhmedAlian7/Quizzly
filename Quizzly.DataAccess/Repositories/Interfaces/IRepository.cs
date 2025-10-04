namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id, string includes = "");
        Task<IEnumerable<T>> GetAllAsync(string includes = "");
        Task<IEnumerable<T>> GetAllAsync(int page = 1, string includes = "");
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();
        Task SaveAsync();

        Task<IEnumerable<T>> GetDeletedAsync();
        void Restore(T entity);
    }
}
