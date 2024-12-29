using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok2.DAL;
using Pustok2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pustok2.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class GenreController : Controller
    {
        private readonly PustokDbContext _context;

        public GenreController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            //var query = _context.Books.AsQueryable();
            var totalpage = _context.Genres.Count()/2d;
            ViewBag.TotalPage = Math.Ceiling(totalpage);
            ViewBag.SelectedPage = page;
            List<Genre> genres = _context.Genres.Include(x=>x.Books).Skip((page-1)*2).Take(2).ToList();
            return View(genres);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //postman kimi proqramlardan http srogusu,basqa saytlardan post sorgusu gele biler
        //fetch uzerinden post sorgusu gonderile biler
        //inspect button alti-value-sil deyeri submit et olmur.
        public IActionResult Create(Genre genre/*string name*/)
        {
            //return Content(name);
            //create genre
            // add to database
            //Genre genre = new Genre()
            //{
            //    Name = name
            //};
            if (!ModelState.IsValid)
            {
                return View();
                return Content("Xeta bas verdi");
            }
            _context.Genres.Add(genre);
            _context.SaveChanges();//deyisiklikleir database-de yadda saxlayir
            TempData["Success"] = "Genre has been created";
            return RedirectToAction("index","dashboard");
        }
        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.Find(id);
            if (genre==null)
            {
                return NotFound();
            }
            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Genre existGenre = _context.Genres.FirstOrDefault(x => x.Id == genre.Id);
            if (existGenre==null)
            {
                return NotFound();
            }
            existGenre.Name = genre.Name;
            _context.SaveChanges();
            TempData["Success"] = "This genre has succesfully been edited";
            return RedirectToAction("index");
            //return Content(genre.Id+""+genre.Name);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(Genre genre)
        //{
        //    Genre existGenre = _context.Genres.FirstOrDefault(x => x.Id == genre.Id);
        //    if (existGenre == null) return NotFound();
        //    existGenre.Name = genre.Name;
        //    _context.SaveChanges();
        //    return RedirectToAction("index");
        //}
        public IActionResult Delete(int id)
        {
            Genre existGenre = _context.Genres.FirstOrDefault(x => x.Id == id);
            if (existGenre==null)
            {
                return Json(new {status=404});
                return NotFound();
            }
            _context.Genres.Remove(existGenre);
            _context.SaveChanges();
            return Json(new { status = 200 });
            return RedirectToAction("index");
        }
    }
}
