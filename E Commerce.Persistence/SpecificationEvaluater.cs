using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence
{
    public static class SpecificationEvaluater
    {

        // _dbContext.Products.Include(P=>P.ProductType).Include(p=>p.ProductBrand)
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(IQueryable<TEntity> EntryPoint,
            ISpecification<TEntity, TKey> specifications) where TEntity : BaseEntity<TKey>
        {
            // _dbContext.Products
            var Query = EntryPoint;

            if (specifications is not null)
            {

                if (specifications.Criteria is not null)
                {
                    Query = Query.Where(specifications.Criteria);
                    //_dbContext.Products.where(p => p.Id == id)
                }


                if (specifications.IncludeExpressions is not null && specifications.IncludeExpressions.Any())
                {
                    /*//foreach (var includeExp in specifications.IncludeExpressions)
                    //{
                    //    Query.Include(includeExp);
                    //}


                    //// Using aggergate instead of foreach
                    /// Aggergate combines list of strings in one string (Ahmed , Mona , Aya , Omar => Ahmed Mona --- Ahmed Mona Aya ---- Ahmed Mona Aya Omar)
*/

                    Query = specifications.IncludeExpressions.Aggregate(Query, (CurrentQuery,
                        includeExp) => CurrentQuery.Include(includeExp));

                    // first iteration
                    // CurentQuery =>  _dbContext.Products
                    // includeExp => (first include) => Include(P=>P.ProductType)
                    // second iteration => _dbContext.Products.Include(P=>P.ProductType)
                    // now CurentQuery becomes : _dbContext.Products.Include(P=>P.ProductType)
                    // and includeExp becomes the second include : Include(p=>p.ProductBrand)
                    // then the result will become :  _dbContext.Products.Include(P=>P.ProductType).Include(p=>p.ProductBrand)




                }

                if (specifications.OrderBy is not null)
                {
                    Query = Query.OrderBy(specifications.OrderBy);
                }

                if (specifications.OrderByDescending is not null)
                {
                    Query = Query.OrderByDescending(specifications.OrderByDescending);
                }

                if (specifications.IsPaginated)
                {
                    Query = Query.Skip(specifications.Skip).Take(specifications.Take);
                }



            }
            // first iteration : _dbContext.Products.Include(P => P.ProductType)
            // second iteration : _dbContext.Products.where(p=>p.BrandId==brandId).Include(P=>P.ProductType).Include(p=>p.ProductBrand).orderby(p=>p.price)
            return Query;
        }
    }
}
