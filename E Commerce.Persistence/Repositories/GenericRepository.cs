using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly StoreDbContext _dbContext;

        public GenericRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(TEntity entity) => await _dbContext.Set<TEntity>().AddAsync(entity);

        public async Task<int> CountAsync(ISpecification<TEntity, TKey> specifications)
        {
            return await SpecificationEvaluater.CreateQuery(_dbContext.Set<TEntity>(), specifications).CountAsync();
        }

        #region GetAll Before using specification

        /* public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = default,
             List<Expression<Func<TEntity, object>>>? Includes = default)
         {
             if (condition is not null)
             {
                 return await _dbContext.Set<TEntity>().Where(condition).ToListAsync();

             }
             // _dbcontext.products ==> Entry point
             if(Includes is not null)
             {
                 IQueryable<TEntity> EnteryPoint = _dbContext.Set<TEntity>();
                 foreach(var includeExp in Includes)
                 {
                     EnteryPoint = EnteryPoint.Include(includeExp);
                 }
                 //_dbcontext.products
                 //_dbcontext.products.Include(p => p.ProductBrand)
                 //_dbcontext.products.Include(p => p.ProductBrand).Include(p => p.ProductType)
                 return await EnteryPoint.ToListAsync();

             }
             return await _dbContext.Set<TEntity>().ToListAsync();
         }*/


        #endregion

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity, TKey> specifications)
        {
            /* IQueryable<TEntity> EntryPoint = _dbContext.Set<TEntity>();
             if (specifications.IncludeExpressions is not null)
             {
                 foreach (var includeExp in specifications.IncludeExpressions)
                 {
                     EntryPoint = EntryPoint.Include(includeExp);
                 }
             }
             return await EntryPoint.ToListAsync();*/

            #region using function instead

            //var Query = SpecificationEvaluater.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications);

            //return await Query.ToListAsync();


            //return await SpecificationEvaluater.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).ToListAsync();
            // will take generic types from parameters :
            return await SpecificationEvaluater.CreateQuery(_dbContext.Set<TEntity>(), specifications).ToListAsync();

            #endregion



        }

        public async Task<TEntity?> GetByIdAsync(TKey id) => await _dbContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity?> GetByIdAsync(ISpecification<TEntity, TKey> specifications)
        {
            return await SpecificationEvaluater.CreateQuery(_dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync();
        }

        public void Remove(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);

        public void update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);

    }
}
