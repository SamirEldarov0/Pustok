using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok2.DAL;
using Pustok2.Models;
using Pustok2.Services;
using Pustok2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
namespace Pustok2.Controllers
{
    public class BookController : Controller
    {
        private readonly LayoutService _layoutService;
        private readonly PustokDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public BookController(PustokDbContext context,UserManager<AppUser> userManager,LayoutService layoutService)
        {
            _layoutService = layoutService;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult ShowIt()
        {
            CommentShowViewModel viewModel = default;
            return Json(viewModel);
        }
        public IActionResult Detail(int id)
        {
            Book book = _context.Books.Include(x=>x.Author).Include(x=>x.Genre).Include(x => x.BookImages)
                .Include(x=>x.BookTags).ThenInclude(x=>x.Tag).Include(x=>x.Comments).ThenInclude(x => x.AppUser)
                .FirstOrDefault(x=>x.Id==id);
            if (book == null) return NotFound();
            //ya ViewBag-eqoymaq olur yada model icerisine
            //ViewBag.RelatedBooks1= _context.Books.Include(x => x.Author)
            //.Include(x => x.Genre).Include(x => x.BookImages).Include(x => x.BookTags).ThenInclude(x => x.Tag)
            //.Where(x => x.GenreId == book.GenreId).ToList();
            List<Book> relatedBooks=_context.Books.Include(x=>x.Author)
            .Include(x=>x.Genre).Include(x=>x.BookImages).Include(x=>x.BookTags).ThenInclude(x=>x.Tag)
            .Include(x=>x.Comments).ThenInclude(x=>x.AppUser)
            .Where(x=>x.GenreId==book.GenreId)
            .ToList();
            ViewBag.RelatedBooks = relatedBooks;
            ViewBag.bookID = book.Id;

            //List<BookCampaign> bookCampaigns = book.BookCampaigns.Where(x => x.BookId == id).ToList();
            //BookCampaign bookCampaign = book.BookCampaigns.FirstOrDefault(x => x.BookId == id);
            //ViewBag.BookCampaigns=bookCampaigns;
            return View(book);
        }
        public IActionResult Indexx(int? genreId,int page=2)
        {
            List<Book> books = _context.Books.Include(x => x.BookImages).Include(x => x.Author).ToList();
            var query = _context.Books.AsQueryable();
            if (query!=null)
            {
                query = query.Where(x => x.GenreId == genreId);
            }
            ViewBag.GenreId = genreId;
            ViewBag.SelectedPage = page ;
            var totalpage = query.Count() / 3d;
            ViewBag.TotalPage = Math.Ceiling(totalpage);
            List<Book> bookList = query.Include(x=>x.Author).Include(x=>x.BookImages).Skip((page-1)*3).Take(3).ToList();
            return View(bookList);
        }

  
        public IActionResult action(int? genreId,double? minPrice,double? maxPrice,string sort,int page=1)
        {
			//actionda sort default olaraq FromAToZ verilib
			List<Book> books = _context.Books.Where(x =>x.GenreId==genreId).ToList();
            var bookList1 = _context.Books.Where(x => x.GenreId == genreId);//Iqqueryable
            books = books.Where(x => x.DiscountedPrice >= minPrice && x.DiscountedPrice <= maxPrice).ToList();
            var query = _context.Books.AsQueryable();
            if (query!=null)
            {
                query = query.Where(x => x.GenreId == genreId);
            }
            if (minPrice!=null&&maxPrice!=null)
            {
                query = query.Where(x => x.DiscountedPrice >= minPrice && x.DiscountedPrice <= maxPrice);
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "FromAToZ":
                        query = query.OrderBy(x => x.Name);
                        break;
                    case "FromZToA":
                        query = query.OrderByDescending(x => x.Name);
                        break;
                    case "LowToHigh":
                        query = query.OrderBy(x=>x.DiscountedPrice);
                        break;
                    case "HighToLow":
                        query = query.OrderByDescending(x => x.DiscountedPrice);
                        break;
                    default:
                        break;
			    }
            }
            //var totalPage = _context.Books.Count() / 3d;
            var totalPage = query.Count() / 3d;
            ViewBag.Sort = sort;
            ViewBag.SelectedMinPrice = minPrice;
            ViewBag.SelectedMaxPrice = maxPrice;
			ViewBag.TotalPage = Math.Ceiling(totalPage);
            ViewBag.Genres = _context.Genres.Include(x=>x.Books).ToList();
            ViewBag.GenreId = genreId;
			//Math.Round(totalPage);
			ViewBag.SelectedPage = page;
			List<Book> bookList = query.Include(x => x.Author).Include(x => x.BookImages).Skip((page - 1) * 3).Take(3).ToList();
            List<Book> bookList2 = _context.Books.Where(x => x.GenreId == genreId).Include(x => x.BookImages).Include(x => x.Author).Skip((page - 1) * 3).Take(3).ToList();
            //return Content($"{ViewBag.TotalPage}");
            return View(bookList);
        }

        public IActionResult GetDetailedBook(int id)
        {
            Book book = _context.Books.Include(x=>x.Comments).Include(x=>x.Author).Include(x=>x.Publisher).Include(x=>x.BookTags).ThenInclude(x=>x.Tag)
                .Include(x=>x.BookImages).FirstOrDefault(x => x.Id == id);
            
            return PartialView("_BookModalPartial",book);
            //var jsonObject = JsonConvert.SerializeObject(book, Formatting.Indented);
            //return Content(jsonObject);
            return Json(book);
        }


        public IActionResult SetSession(int id=3)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);
            var JsonObject=JsonConvert.SerializeObject(book, Formatting.Indented);
            HttpContext.Session.SetString("Book", JsonObject);
            return RedirectToAction("index", "home");
        }


        public IActionResult ShowSession()
        {
            var sessionvalue = HttpContext.Session.GetString("Book");
            var book = JsonConvert.DeserializeObject<Book>(sessionvalue);
            return Json(book);
            return Content(sessionvalue);
        }
        public IActionResult GiveRole()
        {
            AppUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            _userManager.AddToRoleAsync(appUser, "Member");
            return Json(appUser);
        }

        public IActionResult SetCookie(int id)
        {
            var book = _context.Books.FirstOrDefault(x=>x.Id==id);
            var cookieValues = HttpContext.Request.Cookies["BookList"];
            if (cookieValues == null)
            {
                List<Book> books = new List<Book>();
                books.Add(book);
                var jsonObject = JsonConvert.SerializeObject(books, Formatting.Indented);
                HttpContext.Response.Cookies.Append("BookList", jsonObject);
            }
            else
            {
                List<Book> books = JsonConvert.DeserializeObject<List<Book>>(cookieValues);
                books.Add(book);
                var JsonObject = JsonConvert.SerializeObject(books, Formatting.Indented);
                HttpContext.Response.Cookies.Append("BookList", JsonObject);
            }
            return RedirectToAction("index", "home");
        }
        public IActionResult ShowCookie()
        {
            var cookieValue = HttpContext.Request.Cookies["BookList"];
            //var book = JsonConvert.DeserializeObject<Book>(cookieValue);
            //return Json(book);
            return Content(cookieValue);
        }

        public IActionResult DeleteCookie(string key)
        {
            
            HttpContext.Response.Cookies.Delete(key);
            return RedirectToAction("index", "home");
        }  

        public IActionResult Show()
        {
            Genre genre = _context.Genres.FirstOrDefault(x=>x.Id==3);
            List<Book> books = genre.Books.ToList();
            return Json(books);
        }

        public IActionResult DeleteBasket(string bas)
        {
            if (bas!="Basket")
            {
                TempData["Error"] = $"There is no basket with this {bas} name";
                return RedirectToAction("index", "home");
            }
            var basket = HttpContext.Request.Cookies["Basket"];
            if (basket==null)
            {
                TempData["Error"] = "There is no basket to delete";
                return RedirectToAction("index", "home");
            }
            HttpContext.Response.Cookies.Delete(bas);
            TempData["Succeed"] = "Basket has succesfully removed";
            return RedirectToAction("index", "home");

        }

        public IActionResult AddBookToBasket(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                TempData["Error"] = "There is no book with this id in db";
                return RedirectToAction("index", "home");
            }
            if (User.Identity.IsAuthenticated&&_userManager.Users.Any(x=>x.UserName==User.Identity.Name&&x.IsAdmin==false))
            {
                AppUser appUser =_userManager.FindByNameAsync(User.Identity.Name).Result;
                BasketItem basketItem = book.BasketItems.FirstOrDefault(x => x.AppUserId == appUser.Id);
                if (basketItem==null)
                {
                    basketItem = new BasketItem()
                    {
                        AppUserId=appUser.Id,
                        BookId=book.Id,
                        Count=1
                    };
                    book.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                _context.SaveChanges();
            }
            else
            {
                var basket = HttpContext.Request.Cookies["Basket"];
                if (basket==null)
                {
                    List<BasketCookieItem> basketCookieItems = new List<BasketCookieItem>();
                    BasketCookieItem basketCookieItem = new BasketCookieItem()
                    {
                        Id=book.Id,
                        Count=1
                    };
                    basketCookieItems.Add(basketCookieItem);
                    basket = JsonConvert.SerializeObject(basketCookieItems, Formatting.Indented);
                    HttpContext.Response.Cookies.Append("Basket", basket);
                }

                else
                {
                    List<BasketCookieItem> basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItem>>(basket);
                    BasketCookieItem basketCookieItem = basketCookieItems.FirstOrDefault(x => x.Id == book.Id);
                    if (basketCookieItem!=null)
                    {
                        basketCookieItem.Count++;
                    }
                    else
                    {
                        basketCookieItem = new BasketCookieItem()
                        {
                            Id = book.Id,
                            Count = 1
                        };
                        basketCookieItems.Add(basketCookieItem);
                        basket = JsonConvert.SerializeObject(basketCookieItems, Formatting.Indented);
                        HttpContext.Response.Cookies.Append("Basket", basket);
                    }
                }

            }
            TempData["Succeed"] = $"{book.Id} {book.Name} has succesfully been added to basket";
            return RedirectToAction("index", "home");
        }
        public IActionResult ShowBaket()
        {
            var basket = HttpContext.Request.Cookies["Basket"];
            if (basket == null)
            {
                TempData["Error"] = "There is no book in basket";
                return RedirectToAction("index", "home");
            }
            List<BasketCookieItem> basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItem>>(basket);
            return Json(basketCookieItems);
        }
        public IActionResult AddToBasket(int id)
        {
            
            var book = _context.Books.Include(x=>x.BasketItems).FirstOrDefault(x => x.Id == id);

            if (book==null)
            {
                TempData["Error"] = $"There is no book in db with this id - {id}";
                return RedirectToAction("index", "home");
                return NotFound();
            }

            if (User.Identity.IsAuthenticated&&_userManager.Users.Any(x=>x.UserName==User.Identity.Name&&x.IsAdmin==false))
            {
                AppUser appUser =_userManager.FindByNameAsync(User.Identity.Name).Result;
                //var appUser1 = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                BasketItem basketItem = book.BasketItems.FirstOrDefault(x => x.AppUserId == appUser.Id);
                if (basketItem != null)
                {
                    basketItem.Count++;
                }
                else
                {
                    basketItem = new BasketItem()
                    {
                        AppUserId=appUser.Id,
                        BookId=book.Id,
                        Count=1
                    };
                    book.BasketItems.Add(basketItem);
                }
                _context.SaveChanges();
            }
            else
            {
                var basket = HttpContext.Request.Cookies["Basket"];
                if (basket == null)
                {
                    var basketCookieItems = new List<BasketCookieItem>()
                    {
                        new BasketCookieItem()
                        {
                            Id=book.Id,
                            Count=1
                        }
                    };
                    var basketStr = JsonConvert.SerializeObject(basketCookieItems, Formatting.Indented);
                    HttpContext.Response.Cookies.Append("Basket", basketStr);
                }
                else
                {
                    List<BasketCookieItem> basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItem>>(basket);
                    BasketCookieItem basketCookieItem = basketCookieItems.FirstOrDefault(x => x./*Book.*/Id == book.Id/*id*/);
                    if (basketCookieItem == null)
                    {
                        basketCookieItem = new BasketCookieItem()
                        {
                            Id = book.Id,
                            Count = 1,
                            //Book=book
                        };
                        basketCookieItems.Add(basketCookieItem);
                        //basketViewModel.BasketItems.Add(basketItemViewModel) ;
                    }
                    else
                    {
                        basketCookieItem.Count++;
                    }
                    //basketViewModel.Count++;
                    //basketViewModel.TotalPrice += book.DiscountedPrice;
                    var JsonObject = JsonConvert.SerializeObject(basketCookieItems);
                    HttpContext.Response.Cookies.Append("Basket", JsonObject);
                }
            }
            //BasketViewModel basketViewModel = _layoutService.GetBasket();
            //return PartialView("_BasketPartialView", basketViewModel);
            return RedirectToAction("index", "home");
        }
        public IActionResult ShowBasket()
        {
            var JsonObject = HttpContext.Request.Cookies["Basket"];
            if (JsonObject==null)
            {
                TempData["Error"] = "There is no book in basket";
                return RedirectToAction("index", "home");
            }
            var basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItem>>(JsonObject);
            
            //var basketViewModel = JsonConvert.DeserializeObject<BasketViewModel>(JsonObject);
            //return Content(JsonObject);
            return Json(basketCookieItems);
            return Json(basketCookieItems);
        }
        public async Task<IActionResult> AddBookToBasketAsync(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);
            if (book==null)
            {
                TempData["Error"] = "This book doesnt exist in db";
                return RedirectToAction("index", "home");
            }

            if (User.Identity.IsAuthenticated&&_userManager.Users.Any(x=>x.UserName==User.Identity.Name&&!x.IsAdmin))
            {
                AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
                BasketItem basketItem = book.BasketItems.FirstOrDefault(x => x.AppUserId == appUser.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem()
                    {
                        AppUserId=appUser.Id,
                        BookId=book.Id,
                        Count=1
                    };
                }
            }



            return View();
        }
        [Authorize(Roles = "Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CommentCreateViewModel commentVM)
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            AppUser user = _userManager.Users.FirstOrDefault(x=>x.UserName==User.Identity.Name);
            if (user==null)
            {
                //TempData["Error"] = "In order to add comment,a user has to be loged in"; cunki Authorizeolub
                return RedirectToAction("login","account");
            }
            //if (user==null)
            //{
            //    return Content("Get the hell outta here,u fucking dump ass nigga,there's no logined user");
            //}
            //model validdirmi
            //kitab varmi
            //comment varmi
            //if (commentVM.Rate == 0)
            //{
            //    TempData["Error"] = "Please,u have to rate this book on a scale from 1 to 5 to post comment";
            //}
            if (!ModelState.IsValid)
            {
                return RedirectToAction("detail", "book", new { id = commentVM.BookId });
            }
            Book book = _context.Books.Include(x=>x.Comments).FirstOrDefault(x => x.Id == commentVM.BookId);
            if (book==null)
            {
                //TempData["Error"] = "There is no book with this id";
				return RedirectToAction("detail", "book", new { id = commentVM.BookId });
				return NotFound();
            }            
            if (_context.Comments.Any(x=>x.BookId==commentVM.BookId&&x.AppUserId==user.Id))
            {
                //Commentlerin icinde bu userin yazdigi kitab varsa icaze verme
                TempData["Error"] = $"{appUser.FullName} has already added a comment to this book";
                return RedirectToAction("detail", "book", new { id = commentVM.BookId });
            }
            Comment comment = new Comment()
            {
                BookId=commentVM.BookId,
                AppUserId=user.Id,
                Text=commentVM.Text,
                Rate=commentVM.Rate,
                CreatedDate=DateTime.UtcNow
            };
            book.Comments.Add(comment);
            //int sum = 0;
            //int count = 0;
            //foreach (var item in book.Comments)
            //{
            //    sum += item.Rate;
            //    count++;
            //}
            //book.Rate = sum / count;
            //_context.Comments.Add(comment);
            _context.SaveChanges();
            TempData["Succeed"] = $"The comment has successfully added to this book by {appUser.FullName}";
            return RedirectToAction("detail", "book", new { id = commentVM.BookId });
        }
        

        //Yuxarida detail de hell oldu
        //public async Task<IActionResult> ShowComment(int bookId=2)
        //{
        //    Comment comment = _context.Comments.FirstOrDefault(x => x.BookId == bookId);
        //    AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        if (comment.AppUserId==appUser.Id)
        //        {
        //            CommentShowViewModel commentShowVM = new CommentShowViewModel()
        //            {
        //                FullName=appUser.FullName,
        //                Rate=comment.Rate,
        //                Text=comment.Text,
        //                DateTime=comment.CreatedDate
        //            };
        //        }
        //    }

        //    return View();
        //}

        public IActionResult LoadComment(int bookId,int page=1)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == bookId);
            
            if (book==null) { return Json(new { status = 404 }); }
            var comments = _context.Comments.OrderByDescending(x => x.CreatedDate).Include(x=>x.AppUser)
                .Where(x => x.BookId == bookId&&x.Status==true)
                .Take(2*page).ToList();
            //skip silirem - .Skip((page - 1) * 2)   page = 1 => 2 comment page=2 => 4
            return PartialView("_BookComments", comments);
            return Json(new { status = 200, data = comments });
        }

        public async Task<IActionResult> DeleteBookInBasket(int id)
        {
            Book book = _context.Books.Include(x=>x.BasketItems).FirstOrDefault(x => x.Id == id);
            if (book==null)
            {
                return NotFound();
                //TempData["Error"] = $"There is no book in database with this id => {id}";
                //return RedirectToAction("index", "home");
            }
            
            if (User.Identity.IsAuthenticated&&_userManager.Users.Any(x=>x.UserName==User.Identity.Name&&x.IsAdmin==false))
            {
                AppUser appUser =await _userManager.FindByNameAsync(User.Identity.Name);
                BasketItem basketItem = book.BasketItems.FirstOrDefault(x => x.AppUserId == appUser.Id);
                if (basketItem == null) return NotFound();
                if (basketItem.Count==1)
                {
                    book.BasketItems.Remove(basketItem);
                }
                else if (basketItem.Count>1)
                {
                    basketItem.Count--;
                }
                _context.SaveChanges();
            }
            else
            {
                var basket = HttpContext.Request.Cookies["Basket"];
                if (basket == null)
                {
                    return NotFound();
                }

                else if (JsonConvert.DeserializeObject<List<BasketCookieItem>>(basket).Count==1)
                {
                    HttpContext.Response.Cookies.Delete("Basket");
                    TempData["Succeed"] = "Basketde bir dene kitab var idi ve silindi...";
                    return RedirectToAction("index", "home");
                }
                else
                {
                    List<BasketCookieItem> basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItem>>(basket);
                    BasketCookieItem basketCookieItem = basketCookieItems.FirstOrDefault(x => x.Id == book.Id);

                    if (!basketCookieItems.Any(x=>x.Id==book.Id))
                    {
                        TempData["Error"] = $"There is no book with this id => {book.Id} in basket";
                        return RedirectToAction("index", "home");
                    }
                    else if (basketCookieItems.Where(x=>x.Id==book.Id).Count()==1)
                    {
                        basketCookieItems.Remove(basketCookieItems.FirstOrDefault(x=>x.Id==book.Id));
                        
                        //basketCookieItems.RemoveAt(book.Id);
                    }
                    else if(basketCookieItems.Any(x=>x.Id==book.Id&&x.Count>1))
                    {
                        basketCookieItem.Count--;
                        //basketCookieItems.FirstOrDefault(x => x.Id == book.Id).Count--;
                    }
                    basket = JsonConvert.SerializeObject(basketCookieItems);
                }
                HttpContext.Response.Cookies.Append("Basket",basket);
            }

            TempData["Succeed"] = "Kitab basketden silindi";
            return RedirectToAction("index", "home");
        }
    }
}
