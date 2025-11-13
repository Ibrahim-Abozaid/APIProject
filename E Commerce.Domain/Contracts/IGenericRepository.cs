using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        //Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = default , List<Expression<Func<TEntity,object>>>? Includes = default);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity, TKey> specifications);

        Task<TEntity?> GetByIdAsync(TKey id);
        Task<TEntity?> GetByIdAsync(ISpecification<TEntity, TKey> specifications);

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);
        void update(TEntity entity);

        Task<int> CountAsync(ISpecification<TEntity, TKey> specifications);


    }
}
