using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class Product {
        [Key]
        public int ProductCode { get; set; }

        [Required(ErrorMessage = "Please fill in product category")]
        public Category Category{ get; set; }

        [Required(ErrorMessage = "Please fill in product name")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please fill in product description")]
        [MaxLength(200, ErrorMessage = "Product description cannot exceed 200 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please fill in product price")]
        [DisplayName("product price")]
        public double Price { get; set; }

        public byte[] Image { get; set; }

        public bool Active { get; set; }

    }
}
