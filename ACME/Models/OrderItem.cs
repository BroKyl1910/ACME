using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID{ get; set; }
        public Product Product{ get; set; }
        public int Quantity { get; set; }
    }
}