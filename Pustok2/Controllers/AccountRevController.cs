using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok2.DAL;
using Pustok2.Models;
using Pustok2.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Pustok2.Controllers
{
	public class AccountRevController : Controller
	{
		private readonly PustokDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly RoleManager<IdentityResult> _roleManager;

		public AccountRevController(PustokDbContext context,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityResult> roleManager)
        {
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
        }
        public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(UserRegisterViewModel userRegisterViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			if (_userManager.Users.Any(x=>x.NormalizedUserName==userRegisterViewModel.UserName.ToUpper()))
			{
				ModelState.AddModelError("UserName", "This username has already been used,pls,try another one");
				return View();
			}
			if (_userManager.Users.Any(x=>x.NormalizedEmail==userRegisterViewModel.Email.ToUpper()))
			{
				ModelState.AddModelError("Email", "This email has already been used");
				return View();
			}
			AppUser appUser = new AppUser()
			{
				FullName = userRegisterViewModel.FullName,
				UserName=userRegisterViewModel.UserName,
				Email = userRegisterViewModel.Email,
				IsAdmin=false
			};

			IdentityResult result=await _userManager.CreateAsync(appUser, userRegisterViewModel.Password);
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
			return RedirectToAction("index","home");
		}
		[Authorize(Roles="Member")]
		public IActionResult LogOut()
		{
			_signInManager.SignOutAsync();
			return RedirectToAction("index", "home");
		}


		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(UserLoginViewModel userLoginVM)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			AppUser appUser =await _userManager.FindByNameAsync(userLoginVM.UserName);
			if (appUser==null||appUser.IsAdmin)
			{
				ModelState.AddModelError("", "UserName or Password is wrong");
			}
			SignInResult signInResult =await _signInManager.PasswordSignInAsync(appUser, userLoginVM.Password, true, false);
			if (!signInResult.Succeeded)
			{
				ModelState.AddModelError("", "UserName or Password is wrong");
				return View();
			}
			return RedirectToAction("index", "home");
		}
		[Authorize(Roles = "Member")]
		public async Task<IActionResult> Edit()
		{
			AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);

			UserUpdateViewModel userUpdateViewModel = new UserUpdateViewModel()
			{
				FullName=appUser.FullName,
				UserName=appUser.UserName,
				Email=appUser.Email,
				Country=appUser.Country,
				City=appUser.City,
				Address=appUser.Address,
				State=appUser.State
			};
			return View(userUpdateViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(UserUpdateViewModel userUpdateViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (appUser.UserName!=userUpdateViewModel.UserName&&_userManager.Users.Any(x=>x.NormalizedUserName==userUpdateViewModel.UserName.ToUpper()))
			{
				ModelState.AddModelError("UserName", "This user has already been used");
				return View();
			}
			if (appUser.Email!=userUpdateViewModel.Email&&_userManager.Users.Any(x=>x.NormalizedEmail==userUpdateViewModel.Email.ToUpper()))
			{
				ModelState.AddModelError("Email", "This email has already been used");
				return View();
			}

			if (string.IsNullOrEmpty(userUpdateViewModel.Password))
			{
				ModelState.AddModelError("Password", "Password is required");
				return View();
			}
			else
			{
				if (userUpdateViewModel.Password!=userUpdateViewModel.ConfirmPassword)
				{
					ModelState.AddModelError("ConfirmPassword", "Password and ConfirmPassword have to be the same");
					return View();
				}
				IdentityResult result =await _userManager.ChangePasswordAsync(appUser, userUpdateViewModel.CurrentPassword, userUpdateViewModel.Password);

				if (!result.Succeeded)
				{
					foreach (var item in result.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}
					return View();
				}	
			}

			appUser.FullName = userUpdateViewModel.FullName;
			appUser.UserName = userUpdateViewModel.UserName;
			appUser.Email = userUpdateViewModel.Email;
			appUser.City = userUpdateViewModel.City;
			appUser.Country = userUpdateViewModel.Country;
			appUser.State = userUpdateViewModel.State;
			appUser.Address = userUpdateViewModel.Address;
			await _userManager.UpdateAsync(appUser);
			await _signInManager.SignInAsync(appUser, true);
			return RedirectToAction("index","home");
		}
	}
}
