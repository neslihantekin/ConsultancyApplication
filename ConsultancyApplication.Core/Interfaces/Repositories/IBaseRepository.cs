using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultancyApplication.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        // Temel CRUD benzeri operasyonlar

        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        void Remove(T entity);

        // Dilerseniz burada ekstra ortak sorgu metodları da tanımlayabilirsiniz.
        // Örneğin: Task<IEnumerable<T>> FindAsync(Expression<Func<T,bool>> predicate);
    }
}
