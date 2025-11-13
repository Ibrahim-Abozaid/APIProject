using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Shared;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services.Specifications
{
    internal static class ProductSpecificationHelper
    {
        public static Expression<Func<Product,bool>> GetProductCriteria(ProductQueryParams queryParams)
        {
            return p => (!queryParams.brandId.HasValue || p.BrandId == queryParams.brandId.Value)
                   && (!queryParams.typeId.HasValue || p.TypeId == queryParams.typeId.Value)
                   && (string.IsNullOrEmpty(queryParams.search) || p.Name.ToLower().Contains(queryParams.search.ToLower()));
        }
    }
}
