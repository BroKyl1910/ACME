using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ACME.Models
{
    public class ProductShoppingCart
    {
        [Key]
        public int ProductShoppingCartID { get; set; }
        public Product Product{ get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public int Quantity { get; set; }
    }
}
