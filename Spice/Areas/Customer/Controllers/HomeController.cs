using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

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

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var count = db.ShoppingCart.Count(sc => sc.ApplicationUserId == claim.Value);
                HttpContext.Session.SetInt32(StaticDetails.sessionCartCount, count);
            }

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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await db.ShoppingCart
                    .Where(sc => sc.ApplicationUserId == shoppingCart.ApplicationUserId && sc.MenuItemId == shoppingCart.MenuItemId)
                    .FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await db.AddAsync(shoppingCart);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + shoppingCart.Count;
                }

                await db.SaveChangesAsync();

                var count = db.ShoppingCart.Where(sc => sc.ApplicationUserId == shoppingCart.ApplicationUserId).Count();
                HttpContext.Session.SetInt32("sessionCartCounter", count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItemFromDb = await db.MenuItem.Include(mi => mi.Category).Include(mi => mi.SubCategory).FirstOrDefaultAsync(mi => mi.Id == shoppingCart.Id);

                ShoppingCart shoppingCartObj = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id,
                };

                return View(shoppingCartObj);
            }
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
