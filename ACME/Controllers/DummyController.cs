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
        public String Index()
        {
            ACMEDbContext context = new ACMEDbContext();
            Product product = DummyData.CreateProduct();
            context.Products.Add(product);
            context.SaveChanges();
            return "Dummy data added to DB";
        }

        // GET: DummyController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DummyController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DummyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: DummyController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DummyController/Edit/5
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

        // GET: DummyController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DummyController/Delete/5
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
