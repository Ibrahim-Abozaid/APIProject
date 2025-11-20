using E_Commerce.Domain.Entities.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface IBasketRepository
    {
        Task<CustmoerBasket?> GetBasketAsync(string basketId);
        Task<CustmoerBasket?> CreateOrUpdateBasketAsync(CustmoerBasket basket , TimeSpan timeToLive =default);

        Task<bool> DeleteBasketAsync(string basketId);
    
    }
}
