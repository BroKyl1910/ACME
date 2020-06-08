using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int ShoppingCartItemID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}