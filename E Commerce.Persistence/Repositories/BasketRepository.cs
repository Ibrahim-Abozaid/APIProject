using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.BasketModule;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace E_Commerce.Persistence.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;

        public BasketRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }
        public async Task<CustmoerBasket?> CreateOrUpdateBasketAsync(CustmoerBasket basket, TimeSpan timeToLive = default)
        {
            var JsonBasket = JsonSerializer.Serialize(basket);
            var IsCreatedOrUpdated = await _database.StringSetAsync(basket.Id, JsonBasket,
               (timeToLive == default) ? TimeSpan.FromDays(7) : timeToLive);

            if (IsCreatedOrUpdated)
            {
                //var Basket = await _database.StringGetAsync(basket.Id);    // basketId it the key ( redis works key value )
                //return JsonSerializer.Deserialize<CustmoerBasket>(Basket!);

                return await GetBasketAsync(basket.Id);

            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteBasketAsync(string basketId) => await _database.KeyDeleteAsync(basketId);  // basketId it the key ( redis works key value )

        public async Task<CustmoerBasket?> GetBasketAsync(string basketId)
        {
            var Basket = await _database.StringGetAsync(basketId);
            if (string.IsNullOrEmpty(Basket))
                return null;
            else
                return JsonSerializer.Deserialize<CustmoerBasket>(Basket!);
        }
    }
}
