using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ACME.Models
{
    public class ProductOrder
    {
        [Key]
        public int ProductOrderID { get; set; }
        public Product Product{ get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
    }
}
