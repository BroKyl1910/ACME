using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public List<OrderItem> Items{ get; set; }
        public string ShippingAddress { get; set; }
        public string ContactNumber { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime ETA { get; set; }
    }
}
