using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACME.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult> Edit(int id)
        {

            ACMEDbContext context = new ACMEDbContext();
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Edit(int ProductID, string Name, string Description, string Price, IFormFile ProductImage)
        {
            ACMEDbContext context = new ACMEDbContext();
            Product product = context.Products.First(p => p.ProductCode == ProductID);
            product.Name = Name;
            product.Description= Description;
            product.Price = Convert.ToDouble(Price.Replace('.', ','));

            //If image is null, means value of file select wasnt changed
            if (ProductImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    ProductImage.CopyTo(ms);
                    product.Image = ms.ToArray();
                }
            }

            try
            {
                context.Update(product);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Products.Any(e => e.ProductCode == ProductID))
                {
                    return "404. Product not found";
                }
                else
                {
                    throw;
                }
            }
            return "OK";
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


        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ACMEDbContext context = new ACMEDbContext();
            var product = await context.Products.FindAsync(id);
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
