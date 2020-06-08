using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class Stock
    {
        [Key]
        public int StockID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

    }
}
