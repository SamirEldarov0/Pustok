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
	[Authorize(Roles ="SuperAdmin")]
	public class Genre2Controller : Controller
	{
		private readonly PustokDbContext _context;

		public Genre2Controller(PustokDbContext context)
        {
			_context = context;
        }
        public IActionResult Index(int page=1)
		{

			ViewBag.SelectedPage = page;
			ViewBag.TotalPage = Math.Ceiling(_context.Genres.Count() / 2d);
            List<Genre> genres = _context.Genres.Include(x=>x.Books).Skip((page-1)*2).Take(2).ToList();

            return View(genres);
		}

		public IActionResult Create()
		{
			return View();
		}
		[ValidateAntiForgeryToken]
		[HttpPost]
		public IActionResult Create(Genre genre)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			_context.Genres.Add(genre);
			_context.SaveChanges();
			return RedirectToAction("index");
		}


		public IActionResult Edit(int id)
		{
			Genre genre = _context.Genres.FirstOrDefault(x => x.Id == id);
			if (genre == null)
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
			return RedirectToAction("index");
		}

		public IActionResult Delete(int id)
		{
			Genre genre = _context.Genres.FirstOrDefault(x => x.Id == id);
			if (genre == null) return Json(new { status = 404 });

			_context.Genres.Remove(genre);
			_context.SaveChanges();

			return Json(new { status = 200 });

		}
	}
}
