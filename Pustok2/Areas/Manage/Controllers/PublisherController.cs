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
    public class PublisherController : Controller
    {
        private readonly PustokDbContext _context;

        public PublisherController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            List<Publisher> publishers = _context.Publishers.Include(x=>x.Books).Skip((page-1)*2).Take(2).ToList();
            ViewBag.SelectedPage = page;
            ViewBag.TotalPage = Math.Ceiling(_context.Publishers.Count() / 2d);
            
            return View(publishers);
        }

        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Publishers.Add(publisher);
            _context.SaveChanges();
            return RedirectToAction("index");
        }


        public IActionResult Edit(int id)
        {
            Publisher oldPublisher = _context.Publishers.FirstOrDefault(x => x.Id == id);
            if (oldPublisher==null)
            {
                return NotFound();
            }
            return View(oldPublisher);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Publisher oldPublisher = _context.Publishers.FirstOrDefault(x => x.Id == publisher.Id);
            if (oldPublisher==null)
            {
                return NotFound();
            }
            oldPublisher.Name = publisher.Name;
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Publisher willbedeleted = _context.Publishers.FirstOrDefault(x => x.Id == id);
            if (willbedeleted==null)
            {
                return Json(new { status = 404 });
            }
            _context.Publishers.Remove(willbedeleted);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }

    }
}
