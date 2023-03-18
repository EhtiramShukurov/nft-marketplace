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

    public class CollectionController : Controller
    {
        AppDbContext _context { get; }

        public CollectionController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult List(int page = 1)
        {
            PaginateVM<NFTCollection> paginateVM = new PaginateVM<NFTCollection>();
            paginateVM.MaxPageCount = (int)Math.Ceiling((decimal)_context.NFTCollections.Count() / 5);
            paginateVM.CurrentPage = page;
            if (page > paginateVM.MaxPageCount || page < 1) return BadRequest();
            paginateVM.Items = _context.NFTCollections?.Skip((page - 1) * 5).Take(5).Include(n=>n.Category).Include(p => p.Creator).ThenInclude(c=>c.NFTUser).OrderByDescending(n=>n.CreationDate);
            return View(paginateVM);
        }
        public IActionResult Details(int id)
        {
            var nfts =  _context.NFTs.Include(n => n.Metadata).ThenInclude(n => n.Creator).Include(n => n.Owner).OrderByDescending(n => n.Metadata.CreatedDate).Where(n=>n.CollectionId == id);
            return View(nfts);
        }
    }
}
