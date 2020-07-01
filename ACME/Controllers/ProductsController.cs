using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACME.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Controllers
{
    public class ProductsController : Controller
    {
        // GET: ProductsController
        public ActionResult Index()
        {
            ACMEDbContext context = new ACMEDbContext();

            return View(context.Products);
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Create(string Name, string Description, string Price, IFormFile ProductImage)
        {
            Product product = new Product()
            {
                Name = Name,
                Description = Description,
                Price = Convert.ToDouble(Price.Replace('.', ','))
            };

            //https://stackoverflow.com/questions/42741170/how-to-save-images-to-database-using-asp-net-core
            using (var ms = new MemoryStream())
            {
                ProductImage.CopyTo(ms);
                product.Image = ms.ToArray();
            }
            ACMEDbContext context = new ACMEDbContext();

            context.Add(product);
            await context.SaveChangesAsync();
            return "OK";

        }

        // GET: ProductsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        /*Separate validation method so that I can perform backend validation on create and edit forms as I am not using razor, I'm using XMLHttpRequest
          so I need to validate manually*/
        public string Validate([Bind("ProductID,Name,Description,Price")] Product product, IFormFile ProductImage, bool Editing)
        {
            //https://stackoverflow.com/questions/42741170/how-to-save-images-to-database-using-asp-net-core
            if (ProductImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    ProductImage.CopyTo(ms);
                    product.Image = ms.ToArray();
                }
            }

            //Manual checking because cannot bind IFormFile directly to Product object
            /*ProductImage can be null because on loading the edit page, the image display is set but the image input isn't,
             therefore when editing, the product can still have a stored image even if one isn't posted with the form*/
            if (product.Image == null && !Editing)
            {
                return "Please provide an image";
            }

            string[] acceptedImageFileTypes = new string[] { "png", "jpg", "jpeg", "gif" };

            if (ProductImage != null && !acceptedImageFileTypes.Contains(ProductImage.FileName.Split('.')[1]))
            {
                return "Please provide an image of type .png, .jpg, or .gif";
            }

            if (ModelState.IsValid)
            {
                return "OK";
            }

            return ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()[0];
        }


        // GET: ProductsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
