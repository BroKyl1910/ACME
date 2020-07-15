using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ACME.Models
{

    public class User
    {
        [Key]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your surname")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Please select a user type")]
        public UserType UserType { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
