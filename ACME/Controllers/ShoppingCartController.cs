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
    public class ShoppingCartController : Controller
    {
        // POST: ShoppingCarts/Add
        [HttpPost]
        public string Add(int ProductCode, int Quantity)
        {
            ACMEDbContext context = new ACMEDbContext();
            string email = HttpContext.Session.GetString("Email");
            var user = context.Users.Where(u => u.Email == email).Include(u => u.ShoppingCart).First();
            ShoppingCart cart = user.ShoppingCart;

            //Check if user already has item in cart
            if (context.ProductShoppingCarts.Any(psc => psc.Product.ProductCode == ProductCode && psc.ShoppingCart.ShoppingCartID == cart.ShoppingCartID))
            {
                //Add quantity to current product shopping cart entry
                ProductShoppingCart entry = context.ProductShoppingCarts.First(psc => psc.Product.ProductCode == ProductCode && psc.ShoppingCart.ShoppingCartID == cart.ShoppingCartID);
                entry.Quantity += Quantity;
            }
            else
            {
                // create new entry
                ProductShoppingCart productShoppingCart = new ProductShoppingCart
                {
                    Product = context.Products.Find(ProductCode),
                    ShoppingCart = cart,
                    Quantity = Quantity
                };
                context.ProductShoppingCarts.Add(productShoppingCart);
            }

            context.SaveChanges();
            return "Ok";
        }

        // POST: ShoppingCarts/Remove
        [HttpPost]
        public string Remove(int ProductCode)
        {
            ACMEDbContext context = new ACMEDbContext();
            string email = HttpContext.Session.GetString("Email");
            var user = context.Users.Where(u => u.Email == email).Include(u => u.ShoppingCart).First();
            ShoppingCart cart = user.ShoppingCart;

            ProductShoppingCart entry = context.ProductShoppingCarts.First(psc => psc.Product.ProductCode == ProductCode && psc.ShoppingCart.ShoppingCartID == cart.ShoppingCartID);
            context.ProductShoppingCarts.Remove(entry);
            context.SaveChanges();
            return "Ok";
        }

        // GET: ShoppingCart/Details/5
        public ActionResult Details()
        {
            ACMEDbContext context = new ACMEDbContext();
            string email = HttpContext.Session.GetString("Email");
            var user = context.Users.Where(u => u.Email == email).Include(u => u.ShoppingCart).First();
            ShoppingCart cart = user.ShoppingCart;
            var cartItems = context.ProductShoppingCarts.Where(psc => psc.ShoppingCart.ShoppingCartID == cart.ShoppingCartID).Include(psc => psc.Product).ToList();
            cartItems.ForEach(ci => {
                if(context.Stock.First(s => s.Product.ProductCode == ci.Product.ProductCode).Quantity <= 0)
                {
                    ci.Quantity = 0;
                }
            });
            var stockCounts = cartItems.Select(ci => ci.Product.ProductCode).Select(pc => context.Stock.First(s => s.Product.ProductCode == pc)).Select(s => s.Quantity).ToList();

            ViewBag.StockCounts = stockCounts;
            ViewBag.TotalItems = cartItems.Sum(c => c.Quantity);
            ViewBag.GrandTotal = cartItems.Sum(c => (c.Quantity * c.Product.Price));
            ViewBag.CartItems = cartItems;
            return View();
        }


        // POST: ShoppingCart/Edit/5
        [HttpPost]
        public string Edit(int ProductCode, int Quantity)
        {
            ACMEDbContext context = new ACMEDbContext();
            string email = HttpContext.Session.GetString("Email");
            var user = context.Users.Where(u => u.Email == email).Include(u => u.ShoppingCart).First();
            ShoppingCart cart = user.ShoppingCart;
            ProductShoppingCart entry = context.ProductShoppingCarts.First(psc => psc.ShoppingCart.ShoppingCartID == cart.ShoppingCartID && psc.Product.ProductCode == ProductCode);
            entry.Quantity = Quantity;
            context.SaveChanges();
            return "Ok";
        }

        // POST: ShoppingCart/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}