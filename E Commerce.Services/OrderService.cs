using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.OrderModule;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOs.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBasketRepository _basketRepository;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IBasketRepository basketRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _basketRepository = basketRepository;
        }
        public async Task<Result<OrderToReturnDTO>> CreateOrder(OrderDTO orderDTO, string Email)
        {
            var OrderAddress = _mapper.Map<OrderAddress>(orderDTO.Address);
            var Basket = await _basketRepository.GetBasketAsync(orderDTO.BasketId);
            if (Basket == null)
                return Error.NotFound("Basket.NotFound", $"Basket with Id {orderDTO.BasketId} Is Not Found");

            List<OrderItem> OrderItems = new List<OrderItem>();

            foreach (var item in Basket.Items)
            {
                var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (product == null)
                    return Error.NotFound("Product.NotFound", $"Product with Id {item.Id} Is Not Found");

                OrderItems.Add(CreateOrderItem(item, product));

            }

            var DeliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDTO.DeliveryMethodId);
            if (DeliveryMethod == null)
                return Error.NotFound("DeliveryMethod.NotFound", $"Product with Id {orderDTO.DeliveryMethodId} Is Not Found");

            var subTotal = OrderItems.Sum(I => I.Price * I.Quantity);

            var order = new Order()
            {
                Address = OrderAddress,
                DeliveryMethod = DeliveryMethod,
                Subtotal = subTotal,
                Items = OrderItems,
                UserEmail = Email,

            };


            await _unitOfWork.GetRepository<Order, int>().AddAsync(order);
            var result = await _unitOfWork.SaveChangesAsync() > 0;
            if (!result) return Error.Failure("Order.Failure", "Order Can Not Be Created");

            return _mapper.Map<OrderToReturnDTO>(order);


        }

        private static OrderItem CreateOrderItem(Domain.Entities.BasketModule.BasketItem item, Product product)
        {
            return new OrderItem()
            {
                ProductItemOrder = new ProductItemOrder()
                {
                    ProductId = product.Id,
                    PictureUrl = product.PictureUrl,
                    ProductName = product.Name
                },
                Price = product.Price,
                Quantity = item.Quantity
            };
        }
    }
}

