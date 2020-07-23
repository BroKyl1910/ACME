using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ACME.Helpers;
using ACME.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ACME.Controllers
{
    public class OrdersController : Controller
    {
        public ActionResult Index()
        {
            ACMEDbContext context = new ACMEDbContext();
            string email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "Users");
            }
            var user = context.Users.Where(u => u.Email == email).Include(u => u.UserType).First();
            if (user.UserType.UserTypeID == 1)
            {
                // Employee
                var orders = context.Orders;
                var dates = orders.Select(o => o.OrderDate);
                var monthStrings = dates.Select(d => DateTimeHelper.IntToString(d.Month) + " " + d.Year).Distinct();

                ViewBag.HasOrders = context.Orders.Any();
                ViewBag.Months = monthStrings;
                return View("IndexEmployee");
            }
            else
            {
                //Customer
                var orders = context.Orders.Where(o => o.User == user).OrderByDescending(o => o.OrderDate).ToList();
                //var productOrders1 = orders.Select(o => context.ProductOrders.Where(po => po.Order == o)).ToList();
                List<ProductOrder> productOrders = new List<ProductOrder>();
                orders.ForEach(o =>
                {
                    foreach (ProductOrder po in context.ProductOrders.Where(p => p.Order == o).Include(p => p.Product))
                    {
                        productOrders.Add(po);
                    }
                });

                ViewBag.Name = user.Name + " " + user.Surname;
                ViewBag.Orders = orders;
                ViewBag.ProductOrders = productOrders;
                return View("IndexCustomer");
            }
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
            order.OrderDate = DateTime.Now.Date;
            order.ETA = DateTime.Now.AddDays(2).Date;
            order.ShippingAddress = shippingAddress;
            order.User = user;
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            foreach (var item in cartItems)
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

        public string MonthlyOrders(string Month, string Year)
        {
            ACMEDbContext context = new ACMEDbContext();
            int month = DateTimeHelper.StringToInt(Month);
            int year = Convert.ToInt32(Year);

            // All orders for the specific month
            var orders = context.Orders.Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year).ToList();
            var productOrders = new List<ProductOrder>();
            //orders.Select(o => context.ProductOrders.Where(po => po.Order == o));
            orders.ForEach(o =>
            {
                productOrders.AddRange(context.ProductOrders.Where(po => po.Order == o).Include(po => po.Product).ToList());
            });


            List<Category> categories = context.Categories.ToList();
            List<int> orderTotals = new List<int>();

            categories.ForEach(c =>
            {
                var orderTotal = productOrders.Where(po => po.Product.Category == c).Sum(po => po.Quantity);
                orderTotals.Add(orderTotal);
            });

            var xCategories = categories.Select(c => c.Name);
            var yOrders = orderTotals;

            return JsonConvert.SerializeObject(new
            {
                xCategories,
                yOrders
            });

        }
    }
}