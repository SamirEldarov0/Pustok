using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok2.Areas.Manage.ViewModels;
using Pustok2.DAL;
using Pustok2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok2.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AdminController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(PustokDbContext context,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize(Roles="SuperAdmin")]
        public IActionResult Index()
        {
            List<AppUser> adminList = _userManager.Users.Where(x => x.IsAdmin == true&&x.FullName!="Super Admin").ToList();
            return View(adminList);
        }
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult CreateAdmin()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateAdmin(CreateAdminViewModel createAdminVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var adminList = _userManager.Users.Where(x => x.IsAdmin);
            if (adminList.Any(x=>x.NormalizedUserName==createAdminVM.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "This username has been taken");
                return View();
            }
            if (adminList.Any(x=>x.NormalizedEmail==createAdminVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "This email has been taken");
                return View();
            }

                

            AppUser admin = new AppUser()
            {
                FullName = createAdminVM.FullName,
                UserName = createAdminVM.UserName,
                Email = createAdminVM.Email,
                IsAdmin=true
            };
            var result = await _userManager.CreateAsync(admin, createAdminVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(admin, "Admin");
            return RedirectToAction("index","admin");
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Edit(string adminId)
        {
            var admins = _userManager.Users.Where(x => x.IsAdmin == true).ToList();
            AppUser admin = admins.FirstOrDefault(x => x.Id == adminId);
            if (admin == null) { return NotFound(); }
            AdminUpdateViewModel adminUpdateVM = new AdminUpdateViewModel()
            {
                AdminId= adminId,
                FullName=admin.FullName,
                UserName=admin.UserName,
                Email = admin.Email
            };
            return View(adminUpdateVM);
        }
        //[Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUpdateViewModel adminUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser admin = _userManager.Users.FirstOrDefault(x => x.Id == adminUpdateVM.AdminId && x.IsAdmin);
            //if (appUser == null)
            //{
            //    return NotFound();
            //}
            //if (appUser.FullName=="Super Admin")
            //{

            //}

            if (admin.UserName!=adminUpdateVM.UserName&&_userManager.Users.Any(x=>x.NormalizedUserName==adminUpdateVM.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "UserName or Email has already been taken...");
                return View();
            }
            if (admin.Email!=adminUpdateVM.Email&&_userManager.Users.Any(x=>x.NormalizedEmail==adminUpdateVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "UserName or Email has already been taken...");
                return View();
            }

            if (!string.IsNullOrEmpty(adminUpdateVM.Password))
            {
                if (adminUpdateVM.Password!=adminUpdateVM.ConfirmPassword)
                {
                    ModelState.AddModelError("Password", "Password and ConfirmPassword cant be the same");
                    return View();
                }
                var result =await _userManager.ChangePasswordAsync(admin, adminUpdateVM.CurrentPassword, adminUpdateVM.Password);

                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);                      
                    }
                    return View();
                }
            }
            admin.FullName = adminUpdateVM.FullName;
            admin.UserName = adminUpdateVM.UserName;
            admin.Email = adminUpdateVM.Email;
            
            await _userManager.UpdateAsync(admin);
            return RedirectToAction("Index");
        }

        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> Delete(string adminId)
        {
            AppUser admin = _userManager.Users.FirstOrDefault(x => x.IsAdmin && x.Id == adminId);
            if (admin == null) { return NotFound(); }
            await _userManager.DeleteAsync(admin);
            return RedirectToAction("index");
        }

        [Authorize(Roles ="Admin,SuperAdmin")]
        public IActionResult Users()
        {
            var users = _userManager.Users.Where(x => x.IsAdmin).ToList();
            if (users==null)
            {
                return NotFound();
            }
            return View(users);
        }
    }
}
