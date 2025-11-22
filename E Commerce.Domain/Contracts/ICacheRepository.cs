using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface ICacheRepository
    {
        Task<string?> GetAsync(string CacheKey); // take key and return value that matches this key
        Task SetAsyn(string CacheKey, string CaheValue, TimeSpan TimeToLive);
    }
}
