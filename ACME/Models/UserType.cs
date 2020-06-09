using System.ComponentModel.DataAnnotations;

namespace ACME.Models
{
    public class UserType
    {
        [Key]
        public int UserTypeID { get; set; }
        public string Name{ get; set; }
    }
}
