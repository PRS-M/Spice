using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment hostEnvironment;

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            this.db = db;
            this.hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await this.db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }
    }
}