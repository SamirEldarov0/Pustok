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
    public class BookTagController : Controller
    {
        private readonly PustokDbContext _context;

        public BookTagController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.SelectedPage = page;
            ViewBag.TotalPage=Math.Ceiling(_context.BookTags.Count()/4d);
            List<BookTag> bookTags = _context.BookTags.Include(x=>x.Book).Include(x=>x.Tag).Skip((page-1)*4).Take(4).ToList();
            return View(bookTags);
        }

        public IActionResult Create()
        {
            ViewBag.Tags = _context.Tags.ToList();
            List<string> bookNames = new List<string>();
            foreach (var item in _context.Books.ToList())
            {
                bookNames.Add(item.Name);
            }
            ViewBag.Books = bookNames;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookTag bookTag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!_context.Tags.Any(x=>x.Id==bookTag.TagId))
            {
                ModelState.AddModelError("TagId", "This tag doesn't exist...");
                return View();
            }
            if (!_context.Books.Any(x => x.Id == bookTag.BookId))
            {
                ModelState.AddModelError("BookId", "This book doesn't exist...");
                return View();
            }
            _context.BookTags.Add(bookTag);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
