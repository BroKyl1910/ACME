using ACME.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACME.Controllers
{
    public class StockController
    {
        [HttpPost]
        public string Restock(int ProductCode, int Quantity)
        {
            ACMEDbContext context = new ACMEDbContext();
            context.Stock.First(s => s.Product.ProductCode == ProductCode).Quantity += Quantity;
            context.SaveChanges();
            return "Ok";
        }
    }
}
