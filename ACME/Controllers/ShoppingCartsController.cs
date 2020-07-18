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
    public class ShoppingCartsController : Controller
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

        // GET: ShoppingCart/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // POST: ShoppingCart/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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