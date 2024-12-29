using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok2.Models;
using System.Threading.Tasks;

namespace Pustok2.Areas.Manage.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [Area("Manage")]
        //[Authorize(Roles ="SuperAdmin,Admin")]//sign in olmamis hec birbrowser bucontrollerin icerisine buraxmayacaq
        public IActionResult Index()
        {
            //AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);
            //if (!appUser.IsAdmin)
            //{
                
            //    return NotFound();
            //}
            return View();
        }
    }
}
