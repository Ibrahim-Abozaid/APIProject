using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.OrderModule
{
    public class Order : BaseEntity<int>
    {
        public string UserEmail { get; set; } = default!;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public OrderAddress Address { get; set; } = default!;
        public DeliveryMethod DeliveryMethod { get; set; } = default!;
        public ICollection<OrderItem> Items { get; set; } = [];
        public decimal Subtotal { get; set; }
        public decimal GetTotal()=> Subtotal + DeliveryMethod.Price;



    }
}
