using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ACME.Helpers;
using ACME.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ACME.Controllers
{
    public class UsersController : Controller
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
                ViewBag.Error = "Username or password is incorrect";
                return View();
            }

            User user = _context.Users.Where(u => u.Email.ToLower().Equals(email.ToLower())).Include(u=> u.UserType).First();
            bool passwordMatches = BCrypt.CheckPassword(password, user.Password);
            if (passwordMatches)
            {
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Name", user.Name);
                HttpContext.Session.SetInt32("UserType", (int) user.UserType.UserTypeID);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Username or password is incorrect";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Register([Bind("Email,Password,Name,Surname")] User user, int UserType)
        {
            user.Email = user.Email.Trim();
            user.Name = user.Name.Trim();
            user.Surname = user.Surname.Trim();
            user.Password = BCrypt.HashPassword(user.Password, BCrypt.GenerateSalt());
            user.UserType = _context.UserTypes.First(u => u.UserTypeID == UserType);

            ShoppingCart shoppingCart = new ShoppingCart();
            _context.ShoppingCarts.Add(shoppingCart);
            _context.SaveChanges();

            user.ShoppingCart = shoppingCart;
            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Name", user.Name);
            HttpContext.Session.SetInt32("UserType", (int)user.UserType.UserTypeID);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        /*Separate validation method so that I can perform backend validation on create and edit forms as I am not using razor, I'm using XMLHttpRequest
          so I need to validate manually*/
        public string Validate([Bind("Email,Password,Name,Surname")] User user, int UserType, string ConfirmPassword)
        {

            if (_context.Users.Any(u => u.Email.Equals(user.Email)))
            {
                return "Email address is already registered";
            }

            //Manual check for null on passwords to avoid exception on next check
            if (user.Password == null)
            {
                return "Please enter a password";
            }

            if (ConfirmPassword == null)
            {
                return "Please confirm your password";
            }

            if (!ConfirmPassword.Equals(user.Password))
            {
                return "Passwords do not match";
            }

            UserType _userType = _context.UserTypes.Find(UserType);

            user.UserType= _userType;

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(user, null, null);
            if (Validator.TryValidateObject(user, validationContext, validationResults, true))
            {
                return "OK";
            }

            return ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()[0];
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }
    }
}
