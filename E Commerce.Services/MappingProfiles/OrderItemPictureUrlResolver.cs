using AutoMapper;
using E_Commerce.Domain.Entities.OrderModule;
using E_Commerce.Shared.DTOs.OrderDTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services.MappingProfiles
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDTO, string>
    {
        private readonly IConfiguration _configuration;

        public OrderItemPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDTO destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ProductItemOrder.PictureUrl)) return string.Empty;
            if (source.ProductItemOrder.PictureUrl.StartsWith("http")) return source.ProductItemOrder.PictureUrl;


            var BaseUrl = _configuration.GetSection("URLs")["BaseUrl"];

            if (string.IsNullOrEmpty(BaseUrl))
                return string.Empty;

            var PicUrl = $"{BaseUrl}{source.ProductItemOrder.PictureUrl}";

            return PicUrl;

        }
    }
}
