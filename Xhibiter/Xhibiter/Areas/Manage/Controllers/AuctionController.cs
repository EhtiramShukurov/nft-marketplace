using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.ViewModels;
using Microsoft.AspNetCore.Authorization;


namespace Xhibiter.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]

    public class AuctionController : Controller
    {
        AppDbContext _context { get; }

        public AuctionController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            PaginateVM<Auction> paginateVM = new PaginateVM<Auction>();
            paginateVM.MaxPageCount = (int)Math.Ceiling((decimal)_context.Auctions.Count() / 5);
            paginateVM.CurrentPage = page;
            if (page > paginateVM.MaxPageCount || page < 1) return BadRequest();
            paginateVM.Items = _context.Auctions?.Skip((page - 1) * 5).Take(5).Include(a=>a.NFT).Include(a=>a.Winner).ThenInclude(w=>w.NFTUser);
            return View(paginateVM);
        }
    }
}
