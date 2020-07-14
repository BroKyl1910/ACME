using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class Category {
        [Key]
        public int CategoryID { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
