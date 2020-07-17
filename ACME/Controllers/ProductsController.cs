using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACME.Helpers;
using ACME.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ACME.Controllers
{
    public class ProductsController : Controller
    {
        // GET: ProductsController
        public ActionResult Index(int? category, string? q)
        {
            ACMEDbContext context = new ACMEDbContext();

            int selectedCategory = category ?? -1;
            var productsWithCategories = context.Products.Include(p => p.Category).Where(p => p.Active == true);

            IQueryable<Product> productsKeywordFiltered;
            if (q != null)
            {
                productsKeywordFiltered = productsWithCategories.Where(p => p.Name.Contains(q));
            }
            else
            {
                productsKeywordFiltered = productsWithCategories;
            }

            IQueryable<Product> productsCategoryFiltered;
            if (selectedCategory != -1)
            {
                productsCategoryFiltered = productsKeywordFiltered.Where(p => p.Category.CategoryID == selectedCategory);
            }
            else
            {
                productsCategoryFiltered = productsKeywordFiltered;
            }

            bool isLoggedIn = HttpContext.Session.GetString("Email") != null;
            if (isLoggedIn)
            {
                User user = context.Users.Where(u => u.Email.Equals(HttpContext.Session.GetString("Email"))).Include(u => u.UserType).First();
                ViewBag.UserType = user.UserType.UserTypeID;
            }
            else
            {
                ViewBag.UserType = -1;
            }
            ViewBag.Categories = context.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.SelectedCategory = selectedCategory;
            return View(productsCategoryFiltered.ToList());
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int id)
        {
            ACMEDbContext context = new ACMEDbContext();
            Product product = context.Products.Where(p => p.ProductCode == id).Include(p => p.Category).First();
            int stockCount = context.Stock.First(s => s.Product.ProductCode == product.ProductCode).Quantity;
            bool isLoggedIn = HttpContext.Session.GetString("Email") != null;
            if (isLoggedIn)
            {
                User user = context.Users.Where(u => u.Email.Equals(HttpContext.Session.GetString("Email"))).Include(u => u.UserType).First();
                ViewBag.UserType = user.UserType.UserTypeID;
            }
            else
            {
                ViewBag.UserType = -1;
            }
            ViewBag.IsLoggedIn = isLoggedIn;
            ViewBag.Stock = stockCount;
            return View(product);
        }

        // GET: ProductsController/Create
        public ActionResult Create()
        {
            ACMEDbContext context = new ACMEDbContext();
            ViewBag.Categories = context.Categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Create(string Name, string Description, string Price, IFormFile ProductImage, int CategoryID)
        {
            ACMEDbContext context = new ACMEDbContext();

            Product product = new Product()
            {
                Name = Name,
                Description = Description,
                Price = Convert.ToDouble(Price.Replace('.', ',')),
                Category = context.Categories.First(c => c.CategoryID == CategoryID),
                Active = true
            };

            //https://stackoverflow.com/questions/42741170/how-to-save-images-to-database-using-asp-net-core
            using (var ms = new MemoryStream())
            {
                ProductImage.CopyTo(ms);
                product.Image = ms.ToArray();
            }

            context.Add(product);
            await context.SaveChangesAsync();
            return "OK";

        }

        // GET: ProductsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {

            ACMEDbContext context = new ACMEDbContext();
            var product = await context.Products.FindAsync(id);
            var categories = context.Categories.OrderBy(c => c.Name).ToList();
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = product.Category.CategoryID;
            return View(product);
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Edit(int ProductID, string Name, string Description, string Price, IFormFile ProductImage, int CategoryID)
        {
            ACMEDbContext context = new ACMEDbContext();
            Product product = context.Products.First(p => p.ProductCode == ProductID);
            product.Name = Name;
            product.Description = Description;
            product.Price = Convert.ToDouble(Price.Replace('.', ','));
            product.Category = context.Categories.ToList().First(c => c.CategoryID == CategoryID);

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
        public string Validate([Bind("ProductID,Name,Description,Price")] Product product, int CategoryID, IFormFile ProductImage, bool Editing)
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

            if (CategoryID == -1)
            {
                return "Please provide a category";
            }

            ACMEDbContext context = new ACMEDbContext();
            product.Category = context.Categories.First(c => c.CategoryID == CategoryID);

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(product, null, null);
            if (Validator.TryValidateObject(product, validationContext, validationResults, true))
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
            product.Active = false;
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
