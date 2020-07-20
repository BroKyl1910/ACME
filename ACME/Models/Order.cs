using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public User User { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime ETA { get; set; }
    }
}
