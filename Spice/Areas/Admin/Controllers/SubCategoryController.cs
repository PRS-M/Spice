﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ManagerUser)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET INDEX
        public async Task<IActionResult> Index()
        {
            var subCategories = await this.db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }

        // GET - CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await this.db.Category.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await this.db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = this.db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error: Sub Category already exists under " + doesSubCategoryExists.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    this.db.SubCategory.Add(model.SubCategory);
                    await this.db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await this.db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await this.db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage,
            };

            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await (from subCategory in this.db.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        // GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await this.db.SubCategory.SingleOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await this.db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await this.db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = this.db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error: Sub Category already exists under " + doesSubCategoryExists.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    var subCatFromDb = await this.db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCatFromDb.Name = model.SubCategory.Name;

                    await this.db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await this.db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await this.db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage,
            };

            //Another way of persisting an id
            //modelVM.SubCategory.Id = id;
            return View(modelVM);
        }

        // GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await this.db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        // POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await this.db.SubCategory.FindAsync(id);

            if (subCategory == null)
            {
                return View();
            }

            this.db.SubCategory.Remove(subCategory);
            await this.db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await this.db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }
    }
}