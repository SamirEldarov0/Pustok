using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok2.DAL;
using Pustok2.Models;
using Pustok2.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok2.Services
{
    public class LayoutService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly PustokDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LayoutService(PustokDbContext context,IHttpContextAccessor httpContextAccessor,UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _httpContextAccessor= httpContextAccessor;
        }

        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }
        public Setting GetSettings()
        {
            return _context.Settings.FirstOrDefault();
        }
        public List<Brand> GetBrands()
        {
            return _context.Brands.ToList();
        }

        public BasketViewModel BasketViewModel()
        {
            var basket = _httpContextAccessor.HttpContext.Request.Cookies["Basket"];
            BasketViewModel basketViewModel = new BasketViewModel()
            {
                BasketViewItems = new List<BasketItemViewModel>(),
                Count = 0,
                TotalPrice = 0
            };
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated&&_userManager.Users.Any(x=>x.UserName==_httpContextAccessor.HttpContext.User.Identity.Name&&!x.IsAdmin))
            {
                AppUser appUser = _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name).Result;
                var basketItems = _context.BasketItems.Where(x => x.AppUser.UserName == appUser.UserName);
                if (basketItems != null)
                {
                    foreach (var item in basketItems)
                    {
                        BasketItemViewModel basketItemViewModel = new BasketItemViewModel()
                        {
                            Id=item.Id,
                            Book=item.Book,
                            Count=item.Count
                        };
                        basketViewModel.BasketViewItems.Add(basketItemViewModel);
                        basketViewModel.Count++;
                        basketViewModel.TotalPrice += item.Count * item.Book.DiscountedPrice;
                    }
                }
            }

            else
            {
                if (basket != null)
                {
                    var basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItem>>(basket);
                    foreach (var item in basketCookieItems)
                    {
                        Book book = _context.Books.Find(item.Id);
                        BasketItemViewModel basketItemViewModel = new BasketItemViewModel()
                        {
                            Id=item.Id,
                            Book=book,
                            Count=item.Count
                        };
                        basketViewModel.BasketViewItems.Add(basketItemViewModel);
                        basketViewModel.Count++;
                        basketViewModel.TotalPrice += book.DiscountedPrice * item.Count;
                    }
                }
            }
            return basketViewModel;
        }

        public async Task<BasketViewModel> BasketAsync()//12.11.24
        {
            var basket = _httpContextAccessor.HttpContext.Request.Cookies["Basket"];

            BasketViewModel basketViewModel = new BasketViewModel()
            {
                BasketViewItems = new List<BasketItemViewModel>(),
                TotalPrice= 0
            };

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated&&_userManager.Users.Any(x=>x.UserName==_httpContextAccessor.HttpContext.User.Identity.Name&&x.IsAdmin))
            {
                AppUser appUser =await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                List<BasketItem> basketItems = (List<BasketItem>)_context.BasketItems.Include(x => x.AppUser).
                    Include(x => x.Book).ThenInclude(x => x.BookImages).Where(x => x.AppUser.Id == appUser.Id);
                if (basketItems!=null)
                {
                    foreach (var item in basketItems)
                    {
                        Book book = _context.Books.FirstOrDefault(x => x.Id == item.Id);
                        BasketItemViewModel basketItemViewModel = new BasketItemViewModel()
                        {
                            Book = book,
                            Count = item.Count
                        };
                        basketViewModel.BasketViewItems.Add(basketItemViewModel);
                        basketViewModel.TotalPrice = basketItemViewModel.Count * book.DiscountedPrice;
                        basketViewModel.Count++;
                    }
                } 
            }
            else
            {
                if (basket!=null)
                {

                }
            }

            return basketViewModel;
        }

    
        public BasketViewModel GetBasket()
        {
            var basket = _httpContextAccessor.HttpContext.Request.Cookies["Basket"];
            BasketViewModel basketData = new BasketViewModel()
            {
                BasketViewItems=new List<BasketItemViewModel>(),
                TotalPrice=0
            };

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _userManager.Users.Any(x => x.UserName == _httpContextAccessor.HttpContext.User.Identity.Name && x.IsAdmin == false))
            {
                List<BasketItem> basketItems = _context.BasketItems.Include(x => x.AppUser).Include(x=>x.Book).ThenInclude(x=>x.BookImages).
                    Where(x => x.AppUser.UserName == _httpContextAccessor.HttpContext.User.Identity.Name).ToList();
                foreach (var item in basketItems)
                {
                    BasketItemViewModel basketItemVM = new BasketItemViewModel()
                    {
                        
                        Book=item.Book,
                        Count=item.Count,
                    };
                    
                    basketData.TotalPrice += basketItemVM.Book.DiscountedPrice * item.Count;
                    basketData.BasketViewItems.Add(basketItemVM);
                    basketData.Count++;
                }
            }
            else
            {
                if (basket != null)
                {
                    //BasketViewModel basketViewModel = JsonConvert.DeserializeObject<BasketViewModel>(basket);
                    List<BasketCookieItem> basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItem>>(basket);
                    //basketViewModel.TotalPrice = 0;
                    foreach (var item in basketCookieItems)
                    {
                        //item.Book = _context.Books.FirstOrDefault(x => x.Id == item.Id);
                        Book book = _context.Books.Include(x=>x.BookImages).FirstOrDefault(x => x.Id == item.Id);
                        if (book != null)
                        {
                            BasketItemViewModel basketItemViewModel = new BasketItemViewModel()
                            {
                                Id = book.Id,
                                Book = book,
                                Count = item.Count
                            };
                            basketData.TotalPrice += book.DiscountedPrice * item.Count;
                            basketData.BasketViewItems.Add(basketItemViewModel);
                            basketData.Count++;
                        }
                    }
                }
            }
            return basketData;


        }
    }
}
