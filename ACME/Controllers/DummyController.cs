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
    public class DummyController : Controller
    {
        // GET: DummyController
        public String Product()
        {
            ACMEDbContext context = new ACMEDbContext();
            Product product = DummyData.CreateProduct();
            context.Products.Add(product);
            context.SaveChanges();
            return "Dummy data added to DB";
        }

        public String Category()
        {
            ACMEDbContext context = new ACMEDbContext();
            var categories = DummyData.CreateCategories();
            categories.ForEach(category => { context.Categories.Add(category); });
            context.SaveChanges();
            return "Dummy data added to DB";
        }

    }
}
