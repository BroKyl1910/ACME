using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class Product {
        [Key]
        public int ProductCode { get; set; }
        public Category Category{ get; set; }
        public string Name { get; set; }
        public string Descripion { get; set; }
        public double Price { get; set; }
        public bool Active { get; set; }
    }
}
