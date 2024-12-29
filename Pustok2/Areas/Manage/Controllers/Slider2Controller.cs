using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Pustok2.DAL;
using Pustok2.Helpers;
using Pustok2.Models;
using System;
using System.Linq;

namespace Pustok2.Areas.Manage.Controllers
{
    [Area("manage")]
    public class Slider2Controller : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly PustokDbContext _context;

        public Slider2Controller(PustokDbContext context,IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.SelectedPage = page;
            ViewBag.TotalPage = Math.Ceiling(_context.Sliders.Count() / 2d);
            var sliders = _context.Sliders.Skip((page - 1) * 2).Take(2).ToList();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (slider.ImageFile==null)
            {
                ModelState.AddModelError("ImageFile", "Required...");
                return View();
            }
            else
            {
                if (slider.ImageFile.ContentType!="image/jpeg"&&slider.ImageFile.ContentType!="image/png")
                {
                    ModelState.AddModelError("ImageFile", "...");
                    return View();
                }
                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "...");
                    return View();
                }
                slider.Image = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            }

            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        

        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
            if (existSlider==null)
            {
                return NotFound();
            }
            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "...");
                    return View();
                }
                if (slider.ImageFile.Length == 123455)
                {
                    ModelState.AddModelError("ImageFile", "...");
                    return View();
                }
                if (!string.IsNullOrEmpty(existSlider.Image))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);
                }
                string newimg = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
                existSlider.Image = newimg;
            }
            else if (slider.ImageFile == null && !string.IsNullOrEmpty(existSlider.Image))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);
                existSlider.Image = null;
            }
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == id);
            if (existSlider == null) return Json(new {status=404});

            if (!string.IsNullOrEmpty(existSlider.Image))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);
            }
            _context.Sliders.Remove(existSlider);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
