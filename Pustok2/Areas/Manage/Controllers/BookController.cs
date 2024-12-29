using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(PustokDbContext context,IWebHostEnvironment env)
        {
            _context=context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.SelectedPage = page;
            ViewBag.TotalPage = Math.Ceiling(_context.Books.Count() / 4d);
            List<Book> books = _context.Books.Include(x=>x.Comments).Include(x => x.Genre).Include(x => x.Author).Skip((page - 1) * 4).Take(4).ToList();
            return View(books);
        }
        
        public IActionResult Create()
        {
            
            ViewBag.GenreList = _context.Genres.ToList();
            ViewBag.AuthorList = _context.Authors.ToList();
            ViewBag.PublisherList = _context.Publishers.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (!ModelState.IsValid) return View();
            //ViewBag.GenreList = _context.Genres.ToList();     //lazim deil
            //ViewBag.AuthorList = _context.Authors.ToList();
            //ViewBag.PublisherList = _context.Publishers.ToList();
            //ViewBag.Tags = _context.Tags.ToList();

            //ViewBag.UnuploadedImageCount = 0;
            //if (book.GenreId==0)
            //{
            //    ModelState.AddModelError("GenreId", "Genre daxil edilmeyib");
            //    return View();
            //}
            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre movcud deyil");
                return View();
            }
            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author movcud deyil");
                return View();
            }
            if (!_context.Publishers.Any(x => x.Id == book.PublisherId))
            {
                ModelState.AddModelError("PublisherId", "Publisher movcud deyil");
                return View();
            }
            if (_context.Books.Any(x => x.Code == book.Code))
            {
                ModelState.AddModelError("Code", "Book code should be different");
                return View();
            }
            if (book.PosterImageFile == null)
            {
                ModelState.AddModelError("PosterImageFile", "Required ..!");
                return View();
            }
            if (book.HoverPosterImageFile == null)
            {
                ModelState.AddModelError("HoverPosterImageFile", "Required ..!");
                return View();
            }
            if (book.PosterImageFile != null && book.HoverPosterImageFile != null)
            {
                if (book.PosterImageFile.ContentType != "image/jpeg" && book.PosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterImageFile", "Fayl .jpg ve ya .png ola biler!");
                    return View();
                }
                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "Faylin olcusu 2 mb-dan cox ola bilmez");
                    return View();
                }

                if (book.HoverPosterImageFile.ContentType != "image/jpeg" && book.HoverPosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("HoverPosterImageFile", "Fayl ancaq .jpg ve ya .png uzantili ola biler");
                    return View();
                }
                if (book.HoverPosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverPosterImageFile", "Faylin olcusu 2 mb-dan cox ola bilmez");
                    return View();
                }
                if (book.Images != null)
                {
                    foreach (var item in book.Images)
                    {
                        if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                        {
                            ModelState.AddModelError("Images", "Fayl ancaq jpg ve png uzantili olmalidir!!!");
                            //ViewBag.UnuploadedImageCount++;
                            return View();
                        }
                        if (item.Length > 2097152)
                        {
                            ModelState.AddModelError("Images", "Fayl ancaq jpg ve png uzantili olmalidir!!!");
                            //ViewBag.UnuploadedImageCount++;
                            return View();
                        }
                    }
                }
            }
            BookImage posterImage = new BookImage()
            {
                //Id = book.Id, _Save olmayib
                //Book=book, olmuyada biler
                Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImageFile),
                PosterStatus = true
            };
            BookImage hoverImage = new BookImage()
            {
                //Book = book,
                Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImageFile),
                PosterStatus = false
            };
            book.BookImages = new List<BookImage>()
            {
                hoverImage,posterImage
            };
            //book.BookImages.Add(hoverImage);
            //book.BookImages.Add(posterImage);


            if (book.Images != null)
            {
                foreach (var item in book.Images)
                {
                    BookImage bookImage = new BookImage()
                    {
                        Image = FileManager.Save(_env.WebRootPath, "uploads/books", item),
                        PosterStatus = null
                    };
                    book.BookImages.Add(bookImage);
                }
            }

            book.BookTags = new List<BookTag>();
            if (book.TagIds != null)
            {
                foreach (var item in book.TagIds)
                {
                    BookTag bookTag = new BookTag()
                    {
                        Book = book,
                        //BookId=book.Id,save olunmayib
                        TagId = item
                    };
                    book.BookTags.Add(bookTag);
                }
            }

            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Show(int id)
        {
            //Book book = _context.Books.FirstOrDefault(x => x.Id == id);
            //return Content(book.Genre.Name+" "+book.GenreId);
            Book book = _context.Books.Include(x => x.BookImages).Include(x => x.BookTags).FirstOrDefault(x => x.Id == id);
            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();
            List<int> list = book.TagIds;
            return Json(list);
        }

        public IActionResult Edit(int id)
        {
            Book book = _context.Books.Include(x=>x.BookImages).Include(x=>x.BookTags).FirstOrDefault(x=>x.Id==id);
            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();
            if (book==null)
            {
                return NotFound();
            }
            ViewBag.GenreList = _context.Genres.ToList();
            ViewBag.AuthorList = _context.Authors.ToList();
            ViewBag.PublisherList = _context.Publishers.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (!ModelState.IsValid) return View();
            //ViewBag.GenreList = _context.Genres.ToList();
            //ViewBag.AuthorList = _context.Authors.ToList();
            //ViewBag.PublisherList = _context.Publishers.ToList();
            var existBook = _context.Books.Include(x=>x.BookImages).Include(x=>x.BookTags).FirstOrDefault(x => x.Id == book.Id);
            if (existBook==null) { return NotFound(); }
            #region Checking
            if (!_context.Genres.Any(x=>x.Id==book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre yoxdur");
                return View();
            }
            if (!_context.Authors.Any(x=>x.Id==book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author yoxdur");
                return View();
            }
            if (!_context.Publishers.Any(x=>x.Id==book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author yoxdur");
                return View();
            }
            if (_context.Books.Any(x=>x.Code==book.Code))
            {
                ModelState.AddModelError("Code", "Code shouldn't be same");
                return View();
            }
            if (book.PosterImageFile!=null)
            {

                if (book.PosterImageFile.ContentType != "image/jpeg" && book.PosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterImageFile", "Fayl .jpg ve ya .png ola biler!");
                    return View();
                }
                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "Faylin olcusu 2 mb-dan cox ola bilme");
                    return View();
                }
            }
            if (book.HoverPosterImageFile != null)
            {

                if (book.HoverPosterImageFile.ContentType != "image/jpeg" && book.HoverPosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("HoverPosterImageFile", "Fayl ancaq .jpg ve ya .png uzantili ola biler");
                    return View();
                }
                if (book.HoverPosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverPosterImageFile", "Faylin olcusu 2 mb-dan cox ola bilmez");
                    return View();
                }
            }            

            if (book.Images!=null)
            {
                foreach (var item in book.Images)
                {
                    if (item.ContentType!="image/jpeg"&&item.ContentType!="image/png")
                    {
                        ModelState.AddModelError("Images", "There can only be files with png and jpg extensions");
                        return View();
                    }
                    if (item.Length> 2097152)
                    {
                        ModelState.AddModelError("Images", "The length of file doesn't have to be more than 2 MB");
                        return View();
                    }
                }
            }
            #endregion
    
            existBook.Name = book.Name;
            existBook.Desc = book.Desc;
            existBook.ProducingPrice = book.ProducingPrice;
            existBook.Price = book.Price;
            existBook.DiscountedPrice = book.Price;
            existBook.Code = book.Code;
            existBook.GenreId = book.GenreId;
            existBook.AuthorId = book.AuthorId;
            existBook.PublisherId = book.PublisherId;
            existBook.IsAvailable = book.IsAvailable;
            existBook.IsFeatured = book.IsFeatured;
            existBook.IsNew = book.IsNew;
            if (book.PosterImageFile != null)
            {
                string fileName = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImageFile);
                BookImage oldPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true);
                if (oldPoster!=null)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", oldPoster.Image);
                    oldPoster.Image = fileName;
                }
                else
                {
                    oldPoster = new BookImage()
                    {
                        Image = fileName,
                        PosterStatus = true
                    };
                    existBook.BookImages.Add(oldPoster);
                }

                //string stringFileNamePoster = FileManager.Save(_env.WebRootPath,"uploads/books",book.PosterImageFile);
                //string filename = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true).Image;
                //if (!string.IsNullOrWhiteSpace(filename))
                //{
                //    FileManager.Delete(_env.WebRootPath, "uploads/books", filename);
                //}
                ////filename = stringFileNamePoster;
                //existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true).Image = stringFileNamePoster;
            }
            if (book.HoverPosterImageFile != null)
            {
                string fileName = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImageFile);
                BookImage oldPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == false);
                if (oldPoster != null)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", oldPoster.Image);
                    oldPoster.Image = fileName;
                }
                //2979d42f - a02b - 4245 - 8416 - 4b342847bb14elvn.jpeg
                else
                {
                    oldPoster = new BookImage()
                    {
                        Image = fileName,
                        PosterStatus = false
                    };
                    existBook.BookImages.Add(oldPoster);
                }
            }
            //_context.SaveChanges();
            if (book.Images != null)
            {
                var existBookImages = existBook.BookImages.Where(x => x.PosterStatus == null).ToList();
                if (existBookImages != null)
                {
                    foreach (var item in existBookImages)
                    {
                        FileManager.Delete(_env.WebRootPath, "uploads/books", item.Image);
                    }
                }

                existBookImages = new List<BookImage>();

                foreach (var item in book.Images)
                {
                    //FileManager.Save(_env.WebRootPath,"uploads/books",item)
                    BookImage bookImage = new BookImage()
                    {
                        Image = FileManager.Save(_env.WebRootPath, "uploads/books", item),
                        PosterStatus = null
                    };
                    existBookImages.Add(bookImage);
                }
                existBook.BookImages.RemoveAll(x => x.PosterStatus == null);
                existBook.BookImages.AddRange(existBookImages);
            }
            
            //existBook.BookImages= existBook.BookImages.Where(x=>book.ImageIds.Contains(x.Id)).ToList();

            //var existTags = existBook.BookTags.ToList();
            //if (existBook!=null)
            //{
            //    existBook.Book
            //}
            //foreach (var item in existBook.BookTags)
            //{
            //    if (!book.TagIds.Contains(item.TagId))
            //    {

            //    }
            //}
            existBook.BookTags.RemoveAll(x => !book.TagIds.Contains(x.TagId));
            //42-ci deq
            //foreach (var item in book.TagIds)
            //{
            //    existBook.BookTags.RemoveAll(x => x.TagId != item);
            //}
            foreach (var item in book.TagIds)
            {
                //book-1,2,3
                //oldBook-3,4
                BookTag bookTag = existBook.BookTags.FirstOrDefault(x => x.TagId == item);
                if (bookTag==null)
                {
                    bookTag = new BookTag()
                    {
                        BookId=book.Id,
                        TagId=item
                    };
                    existBook.BookTags.Add(bookTag);
                }
                else
                {
                    continue;
                }
            }
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Book book = _context.Books.Find(id);
            if (book == null) return Json(new { status = 404 });
            _context.Books.Remove(book);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }

        public IActionResult CommentBook1(int bookId,int page=1)
        {
            var book = _context.Books.FirstOrDefault(x => x.Id == bookId);
            if (book == null) return NotFound();
            var comments=book.Comments.Skip((page-1)*2).Take(2).ToList();
            ViewBag.SelectedPage=page;
            ViewBag.TotalPage = Math.Ceiling(_context.Comments.Where(x => x.BookId == bookId).Count() / 2d);
            return View(comments);
        }

        public IActionResult CommentAccept1(int commentId)
        {
            Comment comment = _context.Comments.FirstOrDefault(x => x.Id == commentId);
            if (comment==null)
            {
                return NotFound();
            }
            comment.Status = true;
            _context.SaveChanges();
            Book book = _context.Books.FirstOrDefault(x => x.Id == comment.BookId);
            List<Comment> acceptedComments = book.Comments.Where(x => x.Status == true).ToList();

            book.Rate = (acceptedComments.Count == 0) ? 0 :(int) acceptedComments.Average(x => x.Rate);
            _context.SaveChanges();
            return RedirectToAction("commentbook1", comment.BookId);
        }

        public IActionResult CommentBook(int bookId,int page=1)
        {
            if (!_context.Books.Any(x => x.Id == bookId))
            {
                return RedirectToAction("index");
                return NotFound();
            }
            var book = _context.Books.FirstOrDefault(x => x.Id == bookId);

            ViewBag.SelectedPage = page;
            ViewBag.TotalPage = Math.Ceiling(_context.Comments.Include(x => x.AppUser).Where(x => x.BookId == bookId).ToList().Count()/2d);
            ViewBag.BookId = bookId;
            //var comments = _context.Comments.Include(x => x.AppUser).Where(x => x.BookId == bookId).Skip((page-1)*2).Take(2).ToList();
            var comments = book.Comments.Skip((page - 1) * 2).Take(2).ToList();

            return View(comments);
        }

        public IActionResult CommentAccept(int commentId)
        {
            Comment comment = _context.Comments.FirstOrDefault(x => x.Id == commentId);
            if (comment == null) return NotFound();
            comment.Status = true;
            _context.SaveChanges();
            Book book = _context.Books.Include(x=>x.Comments).FirstOrDefault(x => x.Id == comment.BookId);
            var acceptedComments = book.Comments.Where(x => x.Status == true);
            book.Rate =acceptedComments.Count()==0?0:(int)Math.Round(acceptedComments.Average(x => x.Rate));
            _context.SaveChanges();
            return RedirectToAction("commentbook", new { bookId = comment.BookId });
        }

        public IActionResult CommentReject(int commentId)
        {
            Comment comment = _context.Comments.FirstOrDefault(x => x.Id == commentId);
            if (comment == null) return NotFound();
            comment.Status = false;
            _context.SaveChanges();
            Book book = _context.Books.Include(x=>x.Comments).FirstOrDefault(x => x.Id == comment.BookId);
            var rejectedComments=book.Comments.Where(x=> x.Status == false);
            book.Rate =rejectedComments.Count()==0?0:(int)Math.Round(rejectedComments.Average(x => x.Rate));
            _context.SaveChanges();
            return RedirectToAction("commentbook", new { bookId = comment.BookId });
        }

    }
}


