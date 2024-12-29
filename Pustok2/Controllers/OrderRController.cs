using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok2.DAL;
using Pustok2.Models;
using Pustok2.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok2.Controllers
{
    public class OrderRController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderRController(PustokDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Checkout()
        {
            AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);
            OrderCreateViewModel orderCreateViewModel = new OrderCreateViewModel()
            {
                Country = appUser.Country,
                City = appUser.City,
                State = appUser.State,
                Address = appUser.Address,
                BasketItems = _context.BasketItems.Include(x=>x.AppUser).Include(x=>x.Book).ThenInclude(x=>x.BookImages).Where(x => x.AppUserId == appUser.Id).ToList()
            };
            return View(orderCreateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckOut(OrderCreateViewModel orderCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            
            return View();
        }
    }
}
