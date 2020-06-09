using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACME.Helpers;
using ACME.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Controllers
{
    public class UserController : Controller
    {
        ACMEDbContext _context = new ACMEDbContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(IFormCollection formCollection)
        {
            string email = formCollection["email"];
            string password = formCollection["password"];

            if (!_context.Users.Any(u => u.Email.ToLower().Equals(email.ToLower())))
            {
                return View();
            }

            User user = _context.Users.First(u => u.Email.ToLower().Equals(email.ToLower()));
            bool passwordMatches = BCrypt.CheckPassword(password, user.Password);
            if (passwordMatches)
            {
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Name", user.Name);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Register(IFormCollection formCollection)
        {
            string name = formCollection["name"];
            string surname = formCollection["surname"];
            string email = formCollection["email"];
            string password = formCollection["password"];
            string confirmPassword = formCollection["confirmPassword"];
            string userType = formCollection["userType"];

            string bCryptPassword = BCrypt.HashPassword(password, BCrypt.GenerateSalt());

            User user = new User()
            {
                Email = email,
                Name = name,
                Surname = surname,
                Password = bCryptPassword,
                UserType = _context.UserTypes.First(u=> u.Name.ToLower()==userType)
            };

            ShoppingCart shoppingCart = new ShoppingCart();

            _context.ShoppingCarts.Add(shoppingCart);

            _context.SaveChanges();

            user.ShoppingCart = shoppingCart;

            _context.Users.Add(user);

            _context.SaveChanges();

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }
    }
}
