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
	//[Authorize(Roles = "Member")] //bezi userlere(member) Member rolu vrilmeyib

	public class AccountController : Controller
	{
		private readonly PustokDbContext _context;
		private UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AccountController(PustokDbContext context,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}
		public IActionResult Register2()
		{
			return View();
		}
		[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> Register2Async(UserRegister2ViewModel userRegister2ViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			if (_userManager.Users.Any(x=>x.UserName.ToUpper()==userRegister2ViewModel.UserName.ToUpper()))
			{
				ModelState.AddModelError("UserName", "This user has already been used");
				return View();
			}
			if (_userManager.Users.Any(x=>x.Email.ToUpper()==userRegister2ViewModel.Email.ToUpper()))
			{
				ModelState.AddModelError("Email", "This email has already been used");
				return View();
			}
			AppUser appUser = new AppUser()
			{
				FullName=userRegister2ViewModel.FullName,
				UserName=userRegister2ViewModel.UserName,
				Email=userRegister2ViewModel.Email,
				IsAdmin=false
			};
			IdentityResult result=await _userManager.CreateAsync(appUser, userRegister2ViewModel.Password);
			if (!result.Succeeded)
			{
				foreach (var item in result.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}
				return View();
			}
			await _userManager.AddToRoleAsync(appUser, "Member");
			await _signInManager.SignInAsync(appUser, true);
			return RedirectToAction("index", "home");
		}

		public IActionResult Login2()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login2Async(UserLogin2ViewModel userLogin2ViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			if (!_userManager.Users.Any(x=>x.UserName==userLogin2ViewModel.UserName))
			{
				ModelState.AddModelError("UserName", "This user doesnt exist in db");
				return View();
			}
			 
			AppUser appUser=_userManager.Users.FirstOrDefault(x=>x.UserName==userLogin2ViewModel.UserName);
			if (appUser==null||appUser.IsAdmin)
			{
				ModelState.AddModelError("", "UserName or Password is wrong");
				return View();
			}

			var result =await _signInManager.PasswordSignInAsync(appUser, userLogin2ViewModel.Password, true, false);

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Username or Password is wrong");
				return View();
			}
			return RedirectToAction("index", "home");
		}





        public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(UserRegisterViewModel userRegisterVM)
		{
			if (!ModelState.IsValid)//Metoduma post olan modelimi yoxlasin invaliddirse view-a qaytarsin
			{
				return View();
			}
			//if (string.IsNullOrWhiteSpace(userRegisterVM.UserName))
			//{
			//	ModelState.AddModelError("UserName", "Gijdillax username-e nese yaz");
			//	return View();
			//}
			if (_userManager.Users.Any(x=>x.NormalizedUserName==userRegisterVM.UserName.ToUpper()))
			{
				ModelState.AddModelError("UserName", "This UserName has already been used!!!");
				return View();
			}
			if (_userManager.Users.Any(x=>x.NormalizedEmail==userRegisterVM.Email.ToUpper()))
			{
				ModelState.AddModelError("Email", "This email address has already been used,please,try another one!!!");
				return View();
			}
			AppUser appUser = new AppUser()
			{
				UserName=userRegisterVM.UserName,
				Email=userRegisterVM.Email,
				FullName=userRegisterVM.FullName,
				IsAdmin=false
			};
			//IdentityResult result1 = _userManager.CreateAsync(appUser, userRegisterVM.Password).Result;
			IdentityResult result=await _userManager.CreateAsync(appUser, userRegisterVM.Password);
			if (!result.Succeeded)
			{
				foreach (var item in result.Errors)
				{
					
					ModelState.AddModelError("", item.Description);
					//Propertiye deyil,errorlari modelin ozune elave edirem
				}
				return View();
				return Content(result.Errors.FirstOrDefault().Description);
			}
			await _userManager.AddToRoleAsync(appUser, "Member");
			await _signInManager.SignInAsync(appUser, true);
			return RedirectToAction("index","home");
		}
		public async Task<IActionResult> GiveUserRoleAsync()
		{
			AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (appUser == null)
			{
				return NotFound();
			}
			await _userManager.AddToRoleAsync(appUser, "Member");
			return Content($"The member role has succesfully been given to {appUser.FullName}");
		}
		[Authorize(Roles = "Member")]
		public IActionResult Show()
		{
			if (User.Identity.IsAuthenticated)
			{
				return Content(User.Identity.Name);
			}
			return Content(User.Identity.IsAuthenticated.ToString());
		}
		public IActionResult GetInfo()
		{
			string userInfo = "";
			if (User.Identity.IsAuthenticated)
			{
				//AppUser appUser=_userManager.Users.FirstOrDefault(x => User.Identity.IsAuthenticated);
				AppUser appUser = _userManager.Users.FirstOrDefault(x => x.UserName==User.Identity.Name);
				//userInfo = appUser?.UserName + "\n" + appUser?.FullName + "\n" + appUser?.Email;
				return Json(appUser);
				return Content(userInfo);
			}
			return Content(User.Identity.IsAuthenticated.ToString());
		}
		[Authorize(Roles = "Member")]
		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("index","home");
		}
		//[Authorize("Member")]
		public IActionResult Login()
		{
			return View();
		}
        [HttpPost]
		[ValidateAntiForgeryToken]
		//[Authorize(Roles = "Member")]
		public async Task<IActionResult> Login(UserLoginViewModel userLoginViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			AppUser appUser =await _userManager.FindByNameAsync(userLoginViewModel.UserName);
			//AppUser appUser1 = _userManager.Users.FirstOrDefault(x => x.UserName == userLoginViewModel.UserName);

			if (appUser == null||appUser.IsAdmin)
			{
				ModelState.AddModelError("", "User name or password is wrong !!!");
				return View();
			}
			var result = await _signInManager.PasswordSignInAsync(appUser, userLoginViewModel.Password, true, false);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "User name or password is wrong !!!");
                return View();
            }
            return RedirectToAction("index", "home");
		}
		[Authorize(Roles = "Member")]
		public async Task<IActionResult> Edit()
		{
			AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
			UserUpdateViewModel userUpdateViewModel = new UserUpdateViewModel()
			{
				Email=appUser.Email,
				FullName=appUser.FullName,
				UserName=appUser.UserName,
				Country=appUser.Country,
				City=appUser.City,
				State=appUser.State,
				Address=appUser.Address
			};
			return View(userUpdateViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Edit(UserUpdateViewModel userUpdateViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
			//UserName-i deyismek istedinse belke basqa belke db-da hemen userName var
			if (appUser.UserName!=userUpdateViewModel.UserName&&_userManager.Users.Any(x=>x.NormalizedUserName==userUpdateViewModel.UserName.ToUpper()))
			{
				ModelState.AddModelError("UserName", "This username has already been used,pls,try another one!");
				return View();
			}
			if (appUser.Email!=userUpdateViewModel.Email&&_userManager.Users.Any(x=>x.NormalizedEmail==userUpdateViewModel.Email.ToUpper()))
			{
				ModelState.AddModelError("Email", "Email has already been taken...");
				return View();
			}

			if (!string.IsNullOrEmpty(userUpdateViewModel.Password))
			{
				if (userUpdateViewModel.Password!=userUpdateViewModel.ConfirmPassword)
				{
					ModelState.AddModelError("ConfirmPassword", "Password and ConfirmPassword are not the same");
					return View();
				}

				var result =await _userManager.ChangePasswordAsync(appUser, userUpdateViewModel.CurrentPassword, userUpdateViewModel.Password);
				if (!result.Succeeded)
				{
					foreach (var item in result.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}
					return View();
				}
			}
			appUser.UserName = userUpdateViewModel.UserName;
			appUser.Email = userUpdateViewModel.Email;
			appUser.FullName = userUpdateViewModel.FullName;
			appUser.Address = userUpdateViewModel.Address;
			appUser.Country = userUpdateViewModel.Country;
			appUser.City = userUpdateViewModel.City;
			appUser.State = userUpdateViewModel.State;
			await _userManager.UpdateAsync(appUser);
			await _signInManager.SignInAsync(appUser, true);
			return RedirectToAction("index", "home");
		}


		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
		{
			AppUser appUser =await _userManager.FindByEmailAsync(forgotPasswordVM.Email);
			if (appUser == null)
			{
				ModelState.AddModelError("Email", "Email isn't valid");
				return View();
			}
			return View(forgotPasswordVM);
		}
	}
}
