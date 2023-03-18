using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.Areas.Manage.ViewModels;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Xhibiter.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]

    public class NFTController : Controller
    {
        AppDbContext _context { get; }

        public NFTController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            PaginateVM<NFT> paginateVM = new PaginateVM<NFT>();
            paginateVM.MaxPageCount = (int)Math.Ceiling((decimal)_context.NFTs.Count() / 5);
            paginateVM.CurrentPage = page;
            if (page > paginateVM.MaxPageCount || page < 1) return BadRequest();
            paginateVM.Items = _context.NFTs?.Skip((page - 1) * 5).Take(5).Include(n => n.Metadata).ThenInclude(n => n.Creator).Include(n => n.Owner).OrderByDescending(n => n.Metadata.CreatedDate);
            return View(paginateVM);
        }
        public IActionResult Details(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            var nft = _context.NFTs.Include(n=>n.Metadata).FirstOrDefault(n=>n.Id == id);
            if (nft is null) return NotFound();
            NFTDetailsVM details = new()
            {
                Sales = _context.Sales.Include(s => s.From).Include(s => s.To).Where(s => s.NFTId == id),
                Metadata = _context.NFTMetadatas.Include(n=>n.NFT).ThenInclude(n=>n.Owner).Include(n=>n.Creator).FirstOrDefault(n=>n.NFT.Id == id),
                Auctions = _context.Auctions.Include(a=>a.Winner).ThenInclude(w=>w.NFTUser).Where(a=>a.NFTId == id),
            };
            return View(details);
        }
        public IActionResult Sales(int page = 1)
        {
            PaginateVM<Sale> paginateVM = new PaginateVM<Sale>();
            paginateVM.MaxPageCount = (int)Math.Ceiling((decimal)_context.NFTs.Count() / 5);
            paginateVM.CurrentPage = page;
            if (page > paginateVM.MaxPageCount || page < 1) return BadRequest();
            paginateVM.Items = _context.Sales?.Skip((page - 1) * 5).Take(5).Include(s => s.From).Include(s => s.To).Include(s => s.NFT).OrderByDescending(n=>n.Date);
            return View(paginateVM);
        }
    }
}
