using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Pustok2.DAL;
using Pustok2.Models;
using Pustok2.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok2.Areas.Manage.Controllers
{
    [Authorize(Roles ="SuperAdmin,Admin")]
    [Area("manage")]
    public class OrderController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IEmailService _service;
        private readonly IHubContext<PustokHub> _hubContext;

        public OrderController(UserManager<AppUser> userManager,PustokDbContext context,IEmailService service,IHubContext<PustokHub> hubContext)
        {
            _context = context;
            _service = service;
            _hubContext = hubContext;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.SelectedPage=page;
            ViewBag.TotalPage = Math.Ceiling(_context.Orders.Count() / 2d);
            var orders = _context.Orders.Include(x=>x.OrderItems).Include(x=>x.AppUser).Skip((page-1)*2).Take(2).ToList();
            return View(orders);
        }
        public IActionResult Detail(int id)
        {
            Order order = _context.Orders.Include(x=>x.OrderItems).ThenInclude(x=>x.Book).Include(x=>x.AppUser).FirstOrDefault(x => x.Id == id);
            if (order==null)
            {
                return NotFound();
            }
            return View(order);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Detail(Order order)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //    Order existOrder = _context.Orders.FirstOrDefault(x=>x.Id==order.Id);
        //    if (order==null)
        //    {
        //        return NotFound();
        //    }
        //    existOrder.Note = order.Note;
        //    _context.SaveChanges();
        //    return RedirectToAction("rejectorder", "order", new {id=order.Id});

        //}

        public async Task<IActionResult> AcceptOrder(int id,string note)
        {
            Order order = _context.Orders.Include(x=>x.AppUser).FirstOrDefault(x => x.Id == id);
            if (order == null) { return Json(new { status = 404 }); }

            order.Status = true;
            order.AdminNote = note;
            _context.SaveChanges();
            if (order.AppUser.ConnectionId != null)
            {
                await _hubContext.Clients.Client(order.AppUser.ConnectionId).SendAsync("OrderAccept");
            }
            // _service.Send(order.AppUser.Email, "Order accepted", "Your order accepted,total : " + order.TotalPrice);
            //_service.Send();
            return Json(new { status = 200 });
            return RedirectToAction("index", "order");
        }

        public IActionResult RejectOrder(int id,string note)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            if (note=="")
            {
                return Json(new { status = 400 });
            }
            order.Status = false;
            order.AdminNote = note;
            _context.SaveChanges();
          

            return Json(new { status = 200 });
            return RedirectToAction("index", "order");
        }


        //public IActionResult Accept(int id, string note)
        //{
        //    Order order = _context.Orders.Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);
        //    if (order == null)
        //    {
        //        return Json(new { status = 404 });
        //    }
        //    order.Status = true;
        //    order.AdminNote = note;
        //    _context.SaveChanges();
        //    //_service.Send(order.AppUser.Email, "Order accepted", "Your order accepted,total : " + order.TotalPrice);
        //    return Json(new { status = 200 });
        //}
        //public ActionResult Reject(int id, string note)
        //{
        //    Order order = _context.Orders.FirstOrDefault(x => x.Id == id);
        //    if (order == null)
        //    {
        //        return Json(new { status = 404 });
        //    }
        //    if (string.IsNullOrEmpty(note))
        //    {
        //        return Json(new { status = 400 });
        //    }
        //    order.Status = false;
        //    order.AdminNote = note;
        //    _context.SaveChanges();
        //    //return RedirectToAction("detail", "order", new { idd = id });
        //    return Json(new { status = 200 });
        //}
    }
}
