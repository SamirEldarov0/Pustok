using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Pustok2.DAL;
using Pustok2.Helpers;
using Pustok2.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pustok2.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SliderController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly PustokDbContext _context;

        public SliderController(PustokDbContext context,IWebHostEnvironment env)
        {
            _env = env;
            _context=context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.SelectedPage = page;
            ViewBag.TotalPage = _context.Sliders.Count();
            List<Slider> sliders = _context.Sliders.Skip(page-1).Take(1).ToList();
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
            
            if (slider.ImageFile!=null)
            {
                if (slider.ImageFile.ContentType!="image/jpeg" && slider.ImageFile.ContentType!="image/png")
                {
                    ModelState.AddModelError("ImageFile", "Faylin uzantisi ancaq jpg ve png olmalidir !!! ");
                    return View();
                }
                if (slider.ImageFile.Length> 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Faylin olcusu 2 mb-dan boyuk ola bilmez");
                    return View();
                }
                slider.Image = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Required...");
                return View();
            }
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
            //Slider slider = _context.Sliders.Find(id);
            if (slider==null)
            {

                TempData["Error"] = "This slider doesnt exist in db";
                return RedirectToAction("index", "dashboard");
                return NotFound();
            }
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
            Slider existSlider=_context.Sliders.FirstOrDefault(x=>x.Id==slider.Id);
            if (existSlider==null)
            {
                TempData["Error"] = "This slider doesnt exist in db";
                return RedirectToAction("index","dashboard");
            }
            existSlider.Title=slider.Title;
            existSlider.Subtitle = slider.Subtitle;
            existSlider.PriceText = slider.PriceText;

            if (slider.ImageFile!=null)
            {
                if (slider.ImageFile.ContentType!="image/jpeg" && slider.ImageFile.ContentType!="image/png")
                {
                    ModelState.AddModelError("ImageFile", "Fayl ancaq png ve jpg uzantili ola biler");
                    return View();
                }
                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Faylin uzunlugu 2 mb-dan yuxari ola bilme");
                    return View();
                }
                string newFileName = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);

                if (!string.IsNullOrWhiteSpace(existSlider.Image))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);
                }
                existSlider.Image = newFileName;
            }
            //else ifde ya string.IsNullOrWhiteSpace(slider.Image)
            else if (slider.ImageFile==null && !string.IsNullOrWhiteSpace(existSlider.Image))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);
                existSlider.Image = null;
            }
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var existSlider = _context.Sliders.FirstOrDefault(x=>x.Id==id);
            if (existSlider==null)
            {
                return Json(new { status=404 });
            }
            if (!string.IsNullOrWhiteSpace(existSlider.Image))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);
            }
            _context.Sliders.Remove(existSlider);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }

    }
}
