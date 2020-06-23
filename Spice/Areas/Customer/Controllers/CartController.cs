using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;

        [BindProperty]
        public OrderDetailsCart DetailsCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            DetailsCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            DetailsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = db.ShoppingCart.Where(sc => sc.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                DetailsCart.ListCart = cart.ToList();
            }

            foreach (var shoppingCart in DetailsCart.ListCart)
            {
                shoppingCart.MenuItem = await db.MenuItem.FirstOrDefaultAsync(mi => mi.Id == shoppingCart.MenuItemId);
                DetailsCart.OrderHeader.OrderTotal = DetailsCart.OrderHeader.OrderTotal + (shoppingCart.MenuItem.Price * shoppingCart.Count);
                shoppingCart.MenuItem.Description = StaticDetails.ConvertToRawHtml(shoppingCart.MenuItem.Description);
                if (shoppingCart.MenuItem.Description.Length > 100)
                {
                    shoppingCart.MenuItem.Description = shoppingCart.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            DetailsCart.OrderHeader.OrderTotalOriginal = DetailsCart.OrderHeader.OrderTotal;

            return View(DetailsCart);
        }
    }
}
