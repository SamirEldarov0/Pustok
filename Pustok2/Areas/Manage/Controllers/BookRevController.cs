using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Pustok2.DAL;
using Pustok2.Helpers;
using Pustok2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pustok2.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BookRevController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookRevController(PustokDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.SelectedPage = page;
            ViewBag.TotalPage = Math.Ceiling(_context.Books.Count() / 4d);
            var books = _context.Books.Include(x => x.Comments).Include(x => x.Author)
                .Include(x => x.Publisher).Include(x => x.Genre).Include(x=>x.BookTags).ThenInclude(x=>x.Tag).Skip((page-1)*4).Take(4).ToList();

            return View(books);
        }

        public IActionResult Create()
        {
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            //checking
            #region Checking
            if (!_context.Genres.Any(x=>x.Id==book.GenreId))
            {
                ModelState.AddModelError("GenreId", "This genre doesn't exist...");
                return View();
            }
            if (!_context.Publishers.Any(x=>x.Id==book.PublisherId))
            {
                ModelState.AddModelError("PublisherId", "This publisher doesn't exist...");
                return View();
            }
            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "This genre doesn't exist...");
                return View();
            }
            if (_context.Books.Any(x=>x.Code==book.Code))
            {
                ModelState.AddModelError("Code", "This code has already been used,please use another one");
                return View();
            }

            if (book.PosterImageFile==null)
            {
                ModelState.AddModelError("PosterImageFile", "PosterImage is required !!!");
                return View();
            }
            

            if (book.HoverPosterImageFile == null)
            {
                ModelState.AddModelError("HoverPosterImageFile", "HoverImage is required !!!");
                return View();
            }

            if (book.PosterImageFile.ContentType!="image/jpeg"&&book.PosterImageFile.ContentType!="image/png")
            {
                ModelState.AddModelError("PosterImageFile", "The extension of this file should only be either jpg or png !");
                return View();
            }
            if (book.PosterImageFile.Length> 2097152)
            {
                ModelState.AddModelError("PosterImageFile", "The length of posterImage must be under 2 mb");
                return View();
            }

            if (book.HoverPosterImageFile.ContentType != "image/jpeg" && book.HoverPosterImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("HoverPosterImageFile", "The extension of this file should only be either jpg or png !");
                return View();
            }
            if (book.HoverPosterImageFile.Length > 2097152)
            {
                ModelState.AddModelError("HoverPosterImageFile", "The length of HoverPosterImageFile must be under 2 mb");
                return View();
            }

            if (book.Images!=null)
            {
                foreach (var item in book.Images)
                {
                    if (item.ContentType!="image/jpeg"&&item.ContentType!="image/png")
                    {
                        ModelState.AddModelError("Images", "The extension of this file should only be either jpg or png !");
                        return View();
                    }
                    if (item.Length> 2097152)
                    {
                        ModelState.AddModelError("Images", "The length of this file must be under 2 mb");
                        return View();
                    }
                }
            }

            #endregion
            BookImage posterImg = new BookImage()
            {
                Book = book,
                Image=FileManager.Save(_env.WebRootPath,"uploads/books",book.PosterImageFile),
                PosterStatus=true
            };
            BookImage hoverImage = new BookImage()
            {
                Book = book,
                Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImageFile),
                PosterStatus=false
            };
            book.BookImages = new List<BookImage>();
            book.BookImages.Add(posterImg);
            book.BookImages.Add(hoverImage);

            foreach (var item in book.Images)
            {
                BookImage image = new BookImage()
                {
                    Image = FileManager.Save(_env.WebRootPath, "uploads/books", item),
                    PosterStatus = null
                };
                book.BookImages.Add(image);
            }

            book.BookTags = new List<BookTag>();
            if (book.TagIds!=null)
            {
                foreach (var item in book.TagIds)
                {
                    BookTag bookTag = new BookTag()
                    {
                        Book=book,
                        TagId=item
                    };
                    book.BookTags.Add(bookTag);
                }
            }

            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);

            if (book==null)
            {
                return NotFound();
            }
            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();

            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();

            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existBook = _context.Books.FirstOrDefault(x => x.Id == book.Id);
            if (existBook==null)
            {
                return NotFound();
            }
            if (!_context.Genres.Any(x=>x.Id==book.GenreId))
            {
                ModelState.AddModelError("GenreId", "This genre doesn't exist...");
                return View();
            }
            if (!_context.Publishers.Any(x=>x.Id==book.PublisherId))
            {
                ModelState.AddModelError("PublisherId", "This publisher doesn't exist...");
                return View();
            }
            if (!_context.Authors.Any(x=>x.Id==book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "This author doesn't exist...");
                return View();
            }
            if (_context.Books.Any(x=>x.Code==book.Code))
            {
                ModelState.AddModelError("Code", "This code's already been used...");
                return View();
            }
            if (book.PosterImageFile!=null)
            {
                if (book.PosterImageFile.ContentType!="image/jpeg"&&book.PosterImageFile.ContentType!="image/png")
                {
                    ModelState.AddModelError("PosterImageFile", "The extension of the image is wrong");
                    return View();
                }
                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "The length has to be less than 2Mb");
                    return View();
                }
            }
            if (book.HoverPosterImageFile != null)
            {
                if (book.HoverPosterImageFile.ContentType != "image/jpeg" && book.HoverPosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("HoverPosterImageFile", "The extension of the image is wrong");
                    return View();
                }
                if (book.HoverPosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverPosterImageFile", "The length has to be less than 2Mb");
                    return View();
                }
            }
            if (book.Images!=null)
            {
                foreach (var item in book.Images)
                {
                    if (item.ContentType!="image/jpeg"&&item.ContentType!="image/png")
                    {
                        ModelState.AddModelError("Images", "The extension of this file has to be either jpg or png !!!");
                        return View();
                    }
                    if (item.Length> 2097152)
                    {
                        ModelState.AddModelError("Images", "The length of this file has to be less than 2Mb");
                        return View();
                    }
                }
            }
            
            
            if (book.PosterImageFile != null)
            {
                string newImage = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImageFile);
                BookImage olderPoster= existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true);
                if (olderPoster!=null)
                {
                    FileManager.Delete(_env.WebRootPath,"uploads/books",olderPoster.Image);
                }
                olderPoster.Image = newImage;
            }
            if (book.HoverPosterImageFile != null)
            {
                string newImage = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImageFile);
                BookImage olderHover = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == false);
                if (olderHover != null)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", olderHover.Image);
                }
                olderHover.Image = newImage;
            }
            //_context.SaveChanges();
            if (book.Images != null)
            {
                existBook.BookImages = existBook.BookImages.Where(x => x.PosterStatus == null).ToList();
                if (existBook.BookImages!=null)
                {
                    foreach (var item in existBook.BookImages)
                    {
                        FileManager.Delete(_env.WebRootPath, "uploads/books", item.Image);
                    }
                }
                
                existBook.BookImages = new List<BookImage>();

                foreach (var item in book.Images)
                {
                    //FileManager.Save(_env.WebRootPath,"uploads/books",item)
                    BookImage bookImage = new BookImage()
                    {
                        Image = FileManager.Save(_env.WebRootPath, "uploads/books", item),
                        PosterStatus = null
                    };
                    existBook.BookImages.Add(bookImage);
                }
            }



            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
