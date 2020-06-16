using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexVM = new IndexViewModel()
            {
                MenuItem = await this.db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await this.db.Category.ToListAsync(),
                Coupon = await this.db.Coupon.Where(c => c.IsActive).ToListAsync()
            };

            return View(indexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await db.MenuItem.Include(mi => mi.Category).Include(mi => mi.SubCategory).FirstOrDefaultAsync(mi => mi.Id == id);

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id,
            };

            return View(shoppingCart);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
