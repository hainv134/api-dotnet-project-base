using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        T? Get(int id);

        IQueryable<T> Find(Expression<Func<T, bool>> expression);

        IQueryable<T> GetAll();

        void Update(T entity);

        void UpdateRange(T[] entities);

        void Remove(T entity);

        void RemoveRange(T[] entities);

        Task<T> Insert(T entity, bool SaveChange);

        Task<T[]> InsertRange(T[] entities, bool SaveChange);

        public IQueryable<T>? Get(params int[] ids);
    }
}