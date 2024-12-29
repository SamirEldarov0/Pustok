using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok2.DAL;
using Pustok2.Models;
using Pustok2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok2.Controllers
{
	public class OrderController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly PustokDbContext _context;

		public OrderController(UserManager<AppUser> userManager,PustokDbContext context)
        {
			_userManager = userManager;
			_context = context;
        }
		[Authorize(Roles ="Member")]
		public IActionResult Index()
		{
			var orders = _context.Orders.Include(x=>x.OrderItems).Include(x=>x.AppUser).Where(x => x.AppUser.UserName == User.Identity.Name).ToList();

			return View(orders);
		}

		[Authorize(Roles ="Member")]
		public IActionResult Detail(int id)
		{
			Order order=_context.Orders.Include(x=>x.OrderItems).ThenInclude(x=>x.Book).Include(x=>x.AppUser).FirstOrDefault(x=>x.Id==id&&x.AppUser.UserName==User.Identity.Name);
			if (order==null)
			{
				TempData["Error"] = $"This user hasnt ordered a book with this id => {id}";
				return RedirectToAction("index");
			}
			return View(order);

		}


		[Authorize(Roles = "Member")]
		public async Task<IActionResult> CheckOut()
		{
			AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);
			OrderCreateViewModel orderCreateVM = new OrderCreateViewModel()
			{
				City=appUser.City,
				Country=appUser.Country,
				Address=appUser.Address,
				State=appUser.State,
				BasketItems=_context.BasketItems.Include(x=>x.Book).Include(x=>x.AppUser).Where(x=>x.AppUserId==appUser.Id).ToList()
			};
			return View(orderCreateVM);
		}
		[Authorize(Roles ="Member")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CheckOut(OrderCreateViewModel orderCreateViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View();
				return RedirectToAction("checkout");
			}
			AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);
			
			List<BasketItem> basketItems =await _context.BasketItems.Include(x=>x.Book).Where(x => x.AppUserId == appUser.Id).ToListAsync();
			if (basketItems.Count() == 0)
			{
				TempData["Error"] = "Basket bosdur";

				return RedirectToAction("checkout");
			}

            Order order = new Order()
			{
				Address=orderCreateViewModel.Address,
				State=orderCreateViewModel.State,
				City=orderCreateViewModel.City,
				Country= orderCreateViewModel.Country,
				Note=orderCreateViewModel.Note,
				AppUserId=appUser.Id,
				OrderedDate=DateTime.UtcNow,
				OrderItems=new List<OrderItem>()
			};
			foreach (var item in basketItems)
			{
				OrderItem orderItem = new OrderItem()
				{
					BookId=item.BookId,
					Price=item.Book.DiscountedPrice,
					Name=item.Book.Name,
					Count=item.Count
					//OrderId=order.Id
				};
				order.OrderItems.Add(orderItem);
				order.TotalPrice += orderItem.Price * orderItem.Count;
			}
			await _context.Orders.AddAsync(order);
			_context.BasketItems.RemoveRange(basketItems);
			await _context.SaveChangesAsync();
			//ViewBag.OrderSucceded = true;
			TempData["Succeed"] = "Sifaris ugurla heyata kecirildi";
			return RedirectToAction("index", "home");
		}

	}
}
