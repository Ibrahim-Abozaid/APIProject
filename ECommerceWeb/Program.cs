
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Persistence.Data.DataSeed;
using E_Commerce.Persistence.Data.DbContexts;
using E_Commerce.Persistence.Data.IdentityData.DataSeed;
using E_Commerce.Persistence.Data.IdentityData.DbContexts;
using E_Commerce.Persistence.Repositories;
using E_Commerce.Services;
using E_Commerce.Services.MappingProfiles;
using E_Commerce.services_Abstraction;
using ECommerceWeb.CustomMiddleWares;
using ECommerceWeb.Extensions;
using ECommerceWeb.Factories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace ECommerceWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddKeyedScoped<IDataIntializer, DataIntializer>("Defualt");
            builder.Services.AddKeyedScoped<IDataIntializer, IdentityDataIntializer>("Identity");
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //builder.Services.AddAutoMapper(x => x.AddProfile(new ProductProfile()));
            //builder.Services.AddAutoMapper(x => x.AddProfile<ProductProfile>());

            builder.Services.AddAutoMapper(typeof(ServiceAssemblyReference).Assembly);

            //builder.Services.AddAutoMapper(x => x.LicenseKey = "", typeof(ProductProfile).Assembly);

            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddTransient<ProductPictureUrlResolver>();


            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!);
            });


            builder.Services.AddScoped<IBasketRepository, BasketRepository>();

            builder.Services.AddScoped<IBasketService, BasketService>();

            builder.Services.AddScoped<ICacheRepository, CacheRepository>();

            builder.Services.AddScoped<ICacheService, CacheService>();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;
            });


            builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });


            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>();
            builder.Services.AddIdentityCore<ApplicationUser>()
                  .AddRoles<IdentityRole>()
                  .AddEntityFrameworkStores<StoreIdentityDbContext>();


            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            #endregion
            var app = builder.Build();


            #region Data Seed - Apply Migration

            //using var Scope = app.Services.CreateScope();

            //var DbContextService = Scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            //if (DbContextService.Database.GetPendingMigrations().Any())
            //    DbContextService.Database.Migrate();


            //var DataIntializerService = Scope.ServiceProvider.GetRequiredService<IDataIntializer>();
            //DataIntializerService.Intialize();


            //app.MigrateDatabase();
            //app.SeedDatabase();

            await app.MigrateDatabaseAsync();
            await app.MigrateIdentityDatabaseAsync();
            await app.SeedDatabaseAsync();
            await app.SeedIdentityDatabaseAsync();


            #endregion


            #region Configure the HTTP request pipeline. [middlewares]
            // Configure the HTTP request pipeline.

            //app.Use(async (Context, Next) =>
            //{
            //    try
            //    {
            //        await Next.Invoke(Context);
            //    }catch(Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //        Context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //        await Context.Response.WriteAsJsonAsync(new
            //        {
            //            StatusCode = StatusCodes.Status500InternalServerError,
            //            Error = $"An Unexpected Error Occured : {ex.Message} "
            //        });
            //    }
            //});

            app.UseMiddleware<ExceptionHandlerMiddleWare>();



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.MapControllers();
            #endregion

            await app.RunAsync();
        }
    }
}
