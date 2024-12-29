using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok2.DAL;
using Pustok2.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pustok2.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TagController : Controller
    {
        private readonly PustokDbContext _context;

        public TagController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<BookTag> bookTags = _context.BookTags.Include(x=>x.Tag).Include(x=>x.Book).ToList();
            //List<Tag> tags = _context.Tags.Include(x => x.BookTags).ToList();
            
            return View(bookTags);
        }
        public IActionResult Create()
        {
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
            _context.BookTags.Add(bookTag);
            _context.SaveChanges();
            return View("index", "tag");
        }
    }
}
