using Infrastructure.Persistence;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T>? Get(params int[] ids)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type);

            var proprertyInfor = type.GetProperties()
                .Where(p =>
                {
                    var attribute = Attribute.GetCustomAttribute(p, typeof(KeyAttribute))
            as KeyAttribute;

                    if (attribute != null) // This property has a KeyAttribute
                    {
                        return true;
                    }
                    return false;
                })
                .FirstOrDefault();

            if (proprertyInfor == null) return null;

            var memberExpression = Expression.Property(parameter, proprertyInfor);
            var expressions = ids.Select(
                ID => Expression.Equal(memberExpression, Expression.Constant(ID, typeof(int)))
            );
            var body = expressions.Aggregate((pre, next) => Expression.Or(pre, next));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return _dbContext.Set<T>().Where(lambda);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().Where(expression);
        }

        public T? Get(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbContext.Set<T>();
        }

        public async Task<T> Insert(T entity, bool SaveChange)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            if (SaveChange) await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T[]> InsertRange(T[] entities, bool SaveChange)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            if (SaveChange) await _dbContext.SaveChangesAsync();
            return entities;
        }

        public void Remove(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void RemoveRange(T[] entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public void UpdateRange(T[] entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }
    }
}