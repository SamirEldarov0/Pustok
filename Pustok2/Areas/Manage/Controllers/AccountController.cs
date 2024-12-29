using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok2.Areas.Manage.ViewModels;
using Pustok2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok2.Areas.Manage.Controllers
{
    [Area("manage")]
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //public async Task<IActionResult> CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
        //    return Ok();
        //}

        public async Task<IActionResult> GiveUserRole()
        {
            AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);
            await _userManager.AddToRoleAsync(appUser, "Member");
            return Ok();
        }
        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser appUser = new AppUser()
        //    {
        //        UserName = "SuperAdmin",
        //        FullName = "Super Admin",
        //        IsAdmin=true     
        //    };
        //    //if (appUser==null)
        //    //{
        //    //    return Content("Fariz");
        //    //}
        //    var result1=await _userManager.CreateAsync(appUser, "Admin123");
        //    var result2=await _userManager.AddToRoleAsync(appUser, "SuperAdmin");
        //    if (!result1.Succeeded)
        //    {
        //        return Content("_userManager.CreateAsync(appUser, \"Admin123\");");
        //    }
        //    if (!result2.Succeeded)
        //    {
        //        return Content(" _userManager.AddToRoleAsync(appUser, \"SuperAdmin\");");
        //    }
        //    return Ok();
        //}
        [AllowAnonymous]



        public async Task<IActionResult> Login()
        {
            AppUser appUser = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            //AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (appUser!=null&&!appUser.IsAdmin)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("login", "account");
            }
            //if (User.Identity.IsAuthenticated&&!_userManager.Users.FirstOrDefault(x=>x.UserName==User.Identity.Name).IsAdmin)
            //{
            //    return NotFound();
            //}
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser admin = await _userManager.FindByNameAsync(loginVM.UserName);
            if (admin == null||!admin.IsAdmin)
            {
                ModelState.AddModelError("", "Username or password is wrong!!!");
                return View();
            }
            var result =await _signInManager.PasswordSignInAsync(admin, loginVM.Password, true, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is wrong!!!");
                return View();
            }
            return RedirectToAction("index", "dashboard");
        }

        [Authorize(Roles="SuperAdmin,Admin")]
        public async Task<IActionResult> ManageLogOut()
        {           
            await _signInManager.SignOutAsync();
            return RedirectToAction("index","dashboard");
        }
        
        public IActionResult Show()
        {           
            AppUser appUser=_userManager.Users.FirstOrDefault(x=>x.UserName==User.Identity.Name&&x.IsAdmin==true);
            if (appUser!=null)
            {
                return Json(appUser);
            }
            return NotFound();
        }
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> Create(CreateAdminViewModel createAdminVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            //_userManager.Users.Where(x => !x.IsAdmin).Any(x => x.NormalizedUserName == createAdminVM.UserName.ToUpper());

            if (_userManager.Users.Any(x => x.NormalizedUserName == createAdminVM.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "This username has already been used...");
                return View();
            }
            if (_userManager.Users.Any(x=>x.NormalizedEmail==createAdminVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "This email has already been taken...");
                return View();
            }
            AppUser admin = new AppUser()
            {
                FullName=createAdminVM.FullName,
                UserName=createAdminVM.UserName,
                Email=createAdminVM.Email,
                IsAdmin=true
            };
            IdentityResult result =await _userManager.CreateAsync(admin, createAdminVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);//"UserName or Password is wrong..."
				}
                return View();
            }
            await _userManager.AddToRoleAsync(admin, "Admin");
            //await _signInManager.SignInAsync(admin, true);
            return RedirectToAction("index", "dashboard");
        }
        //[Authorize(Roles="SuperAdmin,Admin")]
        //public async Task<IActionResult> Edit()
        //{
        //    //AppUser appUser = _userManager.Users.FirstOrDefault(x=>x.UserName==User.Identity.Name&&x.IsAdmin);
        //    AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);
        //    AdminUpdateViewModel adminUpdateVM = new AdminUpdateViewModel()
        //    {
        //        FullName=appUser.FullName,
        //        UserName=appUser.UserName,
        //        Email=appUser.Email,

        //    };
        //    return View(adminUpdateVM);
        //}
        [Authorize(Roles="SuperAdmin,Admin")]
        public async Task<IActionResult> Update()
        {
            AppUser admin=await _userManager.FindByNameAsync(User.Identity.Name);
            AdminUpdateViewModel adminUpdateVM = new AdminUpdateViewModel()
            {
                FullName = admin.FullName,
                UserName=admin.UserName,
                Email=admin.Email
            };
            return View(adminUpdateVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(AdminUpdateViewModel adminUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            
            AppUser admin =await _userManager.FindByNameAsync(User.Identity.Name);
            if (admin.UserName != adminUpdateVM.UserName&&_userManager.Users.Any(x=>x.NormalizedUserName==adminUpdateVM.UserName.ToUpper()))
            {              
                ModelState.AddModelError("UserName", "This username has already been used");
                return View();
            }
            if (admin.Email!=adminUpdateVM.Email&&_userManager.Users.Any(x=>x.NormalizedEmail==adminUpdateVM.UserName.ToUpper()))
            {
                ModelState.AddModelError("Email", "This email has already been used");
                return View();
            }

            if (!string.IsNullOrEmpty(adminUpdateVM.Password))
            {
                if (adminUpdateVM.Password!=adminUpdateVM.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and CondirmPassword must be the same");
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
            if (admin.UserName=="SuperAdmin")
            {
                admin.FullName = adminUpdateVM.FullName;
                admin.UserName = adminUpdateVM.UserName;
                admin.Email = adminUpdateVM.Email;
            }

            await _userManager.UpdateAsync(admin);

            return RedirectToAction("Index", "Dashboard");
        }

        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Edit()
        {
            AppUser currentAdmin = await _userManager.FindByNameAsync(User.Identity.Name);
            AdminUpdateViewModel adminUpdateViewModel = new AdminUpdateViewModel()
            {
                FullName=currentAdmin.FullName,
                UserName=currentAdmin.UserName,
                Email=currentAdmin.Email,
                AdminId=currentAdmin.Id
            };
            return View(adminUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(AdminUpdateViewModel adminUpdateViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser admin =await _userManager.FindByNameAsync(adminUpdateViewModel.UserName);//User.Identity.Name-de olar..?

            IdentityResult result =await _userManager.ChangePasswordAsync(admin, adminUpdateViewModel.CurrentPassword, adminUpdateViewModel.Password);
            if (result.Succeeded==false)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            admin.UserName = adminUpdateViewModel.UserName;
            admin.Email = adminUpdateViewModel.Email;
            await _userManager.UpdateAsync(admin);
            await _signInManager.SignInAsync(admin,true);
            return RedirectToAction("index", "dashboard");
        }



    }
}
