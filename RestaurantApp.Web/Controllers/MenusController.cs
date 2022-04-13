using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestaurantApp.Entities;
using RestaurantApp.Web.Models;

namespace RestaurantApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MenusController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:44366/api");
        HttpClient client;

        private readonly RestaurantDbContext _context;

        public MenusController(RestaurantDbContext context)
        {
            _context = context;

            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }

        // GET: Menus
        public IActionResult Index()
        {
            try
            {

                var meals = _context.Menus.Include(m => m.Chef).ToList();
                return View(meals);

            }
            catch (Exception ex)
            {
                Logger.LogWriter.LogException(ex);
                return NotFound();
            }
        }

        // GET: Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(m => m.Chef)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Menus/Create
        public IActionResult Create()
        {
            ViewData["ChefId"] = new SelectList(_context.Chefs, "Id", "Name");

            List<string> mlist = new List<string>();
            ViewBag.Meal = new SelectList(_context.Meals, "Id", "Name");
            foreach (var item in ViewBag.Meal)
            {
                mlist.Add(item.Text + ", ");
            }
            ViewBag.Meals = mlist;

            return View();
        }

        // POST: Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ChefId,Price,Meals")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        // GET: Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            ViewData["ChefId"] = new SelectList(_context.Chefs, "Id", "Name", menu.ChefId);
            return View(menu);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ChefId,Price,Meals")] Menu menu)
        {

            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        // GET: Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Meals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }


        [HttpGet]
        [Route("GetMenus")]
        public IActionResult GetMenus()
        {
            var meals = _context.Menus.Include(m => m.Chef);
            return new OkObjectResult(meals);
        }

        [HttpGet]
        [Route("GetMenuById")]
        public IActionResult GetMenuById(int id)
        {

            var meal = from a in _context.Menus.Where(p => p.Id == id).ToList()
                       select new
                       {
                           a.Id,
                           a.Name,
                           a.ChefId,
                           a.Price,
                           a.Meals

                       };
            return new OkObjectResult(meal);
        }

        [HttpPost]
        [Route("CreateMenu")]
        public IActionResult CreateMenu([FromBody] Menu menu)
        {
            _context.Add(menu);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Menu), new { id = menu.Id }, menu);
        }

        [HttpPut]
        [Route("UpdateMenu")]
        public IActionResult UpdateMenu([FromBody] Menu menu)
        {
            _context.Update(menu);
            _context.SaveChanges();
            return new OkResult();
        }

        [HttpDelete]
        [Route("DeleteMenu")]
        public IActionResult DeleteMenu(int id)
        {
            var menu = _context.Menus.Find(id);
            _context.Menus.Remove(menu);
            _context.SaveChanges();
            return new OkResult();
        }

    }
}
