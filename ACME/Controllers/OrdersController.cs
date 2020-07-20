using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACME.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ACME.Controllers
{
    public class OrdersController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            ACMEDbContext context = new ACMEDbContext();
            string email = HttpContext.Session.GetString("Email");
            var user = context.Users.Where(u => u.Email == email).Include(u => u.ShoppingCart).First();
            ShoppingCart cart = user.ShoppingCart;
            var cartItems = context.ProductShoppingCarts.Where(psc => psc.ShoppingCart.ShoppingCartID == cart.ShoppingCartID).Include(psc => psc.Product).ToList();
            List<string> provinces = new List<string> {
                "Eastern Cape",
                "Free State",
                "Gauteng",
                "KwaZulu-Natal",
                "Limpopo",
                "Mpumalanga",
                "Northern Cape",
                "North West",
                "Western Cape"
            };

            ViewBag.TotalItems = cartItems.Sum(c => c.Quantity);
            ViewBag.GrandTotal = cartItems.Sum(c => (c.Quantity * c.Product.Price));
            ViewBag.Provinces = provinces;
            return View();
        }

        [HttpPost]
        public async Task<string> Checkout(string StreetAddress, string? Complex, string Suburb, string City, string Province, string PostCode)
        {
            ACMEDbContext context = new ACMEDbContext();
            string email = HttpContext.Session.GetString("Email");
            var user = context.Users.Where(u => u.Email == email).Include(u => u.ShoppingCart).First();
            ShoppingCart cart = user.ShoppingCart;
            var cartItems = context.ProductShoppingCarts.Where(psc => psc.ShoppingCart.ShoppingCartID == cart.ShoppingCartID).Include(psc => psc.Product).ToList();

            string shippingAddress = StreetAddress + ", " + ((Complex != null) ? (Complex + ", ") : "") + Suburb + ", " + City + ", " + Province + ", " + PostCode;
            Order order = new Order();
            order.ETA = DateTime.Now.AddDays(2).Date;
            order.ShippingAddress = shippingAddress;
            order.User = user;
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            foreach(var item in cartItems)
            {
                ProductOrder po = new ProductOrder();
                po.Order = order;
                po.Product = item.Product;
                po.Quantity = item.Quantity;

                context.Stock.First(s => s.Product == item.Product).Quantity -= item.Quantity;
                context.ProductOrders.Add(po);
                context.ProductShoppingCarts.Remove(item);
            }

            await context.SaveChangesAsync();

            return "Ok";
        }
    }
}