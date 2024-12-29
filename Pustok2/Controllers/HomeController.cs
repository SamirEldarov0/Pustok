using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok2.DAL;
using Pustok2.Models;
using Pustok2.ViewModels;
using System.Linq;

namespace Pustok2.Controllers
{
    public class HomeController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(PustokDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel()
            {
                Sliders=_context.Sliders.OrderBy(x=>x.Order).ToList(),
                Features=_context.Features.OrderBy(x=>x.Order).ToList(),
                UpPromotions=_context.UpPromotions.ToList(),
                DownPromotion=_context.DownPromotions.FirstOrDefault(),
                FeaturedBooks=_context.Books.Include(x=>x.Author).Include(x=>x.Publisher).Include(x=>x.Genre).Include(x=>x.BookImages)
                .Where(x=>x.IsFeatured).ToList(),
                NewBooks=_context.Books.Include(x=>x.Author).Include(x=>x.Publisher).Include(x=>x.Genre).Include(x=>x.BookImages).Where(x=>x.IsNew).ToList(),
                AvailableBooks=_context.Books.Include(x => x.Author).Include(x => x.Publisher).Include(x => x.Genre).Include(x => x.BookImages).Where(x=>x.IsAvailable).ToList(),
                Setting=_context.Settings.FirstOrDefault()
            };
            //var FeaturesList = _context.Features.Where(x => x.Order > 2).ToList();
            //var isExist = _context.Features.Any(x => x.Order == 2);
            //var sum = _context.Features.Sum(x => x.Order);
            //var slider = _context.Sliders.Find(1);
            //var FeaturesList1 = _context.Features.Where(x => x.Order > 2).Skip(10).Take(5).ToList();

            return View(homeViewModel);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult Register(UserRegisterViewModel userRegisterVM)
		{
            if (!ModelState.IsValid)//Metoduma post olan modelimi yoxlasin invaliddirse view-a qaytarsin
            {
				return View();

			}
            return RedirectToAction("index");
		}

	}
}
