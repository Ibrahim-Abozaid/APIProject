using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Data.DataSeed
{
    public class DataIntializer : IDataIntializer
    {
        private readonly StoreDbContext _dbContext;

        public DataIntializer(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task IntializeAsync()
        {
            try
            {
                var HasProducts = await _dbContext.Products.AnyAsync();
                var HasBrands = await _dbContext.ProductBrands.AnyAsync();
                var HasTypes = await _dbContext.ProductTypes.AnyAsync();

                if (HasProducts && HasBrands && HasTypes) return;


                if (!HasBrands)
                {
                    await SeedDataFromJsonAsync<ProductBrand, int>("brands.json", _dbContext.ProductBrands);
                }
                if (!HasTypes)
                {
                    await SeedDataFromJsonAsync<ProductType, int>("types.json", _dbContext.ProductTypes);
                }
                await _dbContext.SaveChangesAsync();
                if (!HasProducts)
                {
                    await SeedDataFromJsonAsync<Product, int>("products.json", _dbContext.Products);
                }
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Data Seed is Faild : {ex} ");
            }




        }


        private async Task SeedDataFromJsonAsync<T, Tkey>(string FileName, DbSet<T> dbSet) where T : BaseEntity<Tkey>
        {
            //E:\Route BackEnd\08 ASP .Net API\ECommerceWebSolution\E Commerce.Persistence\Data\DataSeed\JSONFiles\

            var FilePath = @"..\E Commerce.Persistence\Data\DataSeed\JSONFiles\" + FileName;

            if (!File.Exists(FilePath)) throw new FileNotFoundException($"File {FileName} is not exist");
            try
            {
                using var dataStreams = File.OpenRead(FilePath);

                var data = await JsonSerializer.DeserializeAsync<List<T>>(dataStreams, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data is not null)
                    await dbSet.AddRangeAsync(data);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error While Reading Json File : {ex} ");
                return;
            }


        }


    }
}
