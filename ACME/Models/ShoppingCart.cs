using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class ShoppingCart {
        [Key]
        public int ShoppingCartID { get; set; }
    }
}
