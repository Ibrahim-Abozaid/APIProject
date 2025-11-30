using E_Commerce.services_Abstraction;
using E_Commerce.Shared;
using E_Commerce.Shared.DTOs.ProductDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Presentation.Controllers
{

    public class ProductsController : ApiBaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        //  Get : BaseUrl/api/Products
        public async Task<ActionResult<PaginatedResult<ProductDTO>>> GetProducts([FromQuery] ProductQueryParams queryParams)
        {
            var Products = await _productService.GetAllProductsAsync(queryParams);

            return Ok(Products);
        }

        [HttpGet("{id}")]
        //  Get : BaseUrl/api/Products/{id}
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {

            var product = await _productService.GetProductByIdAsync(id);
            return HandleResult<ProductDTO>(product);

        }

        [HttpGet("brands")]
        //  Get : BaseUrl/api/Products/brands
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetAllBrands()
        {
            var Brands = await _productService.GetAllBrandsAsync();

            return Ok(Brands);
        }

        [HttpGet("types")]
        //  Get : BaseUrl/api/Products/types
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetAllTypes()
        {
            var Types = await _productService.GetAllTypesAsync();
            return Ok(Types);
        }


    }
}
