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

    public class UserController : Controller
    {
        AppDbContext _context { get; }

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            PaginateVM<UserProfile> paginateVM = new PaginateVM<UserProfile>();
            paginateVM.MaxPageCount = (int)Math.Ceiling((decimal)_context.UserProfiles.Count() / 5);
            paginateVM.CurrentPage = page;
            if (page > paginateVM.MaxPageCount || page < 1) return BadRequest();
            paginateVM.Items = _context.UserProfiles?.OrderByDescending(u=>u.CreatedDate).Skip((page - 1) * 5).Take(5).Include(p => p.NFTUser);
            return View(paginateVM);
        }
        public IActionResult Delete(int? id)
        {
            return View();
        }
        public IActionResult Details(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            var user = _context.UserProfiles.Include(u=>u.NFTUser).FirstOrDefault(n=>n.Id == id);
            if (user is null) return NotFound();
            UserDetailVM details = new UserDetailVM()
            {
                Collections = _context.NFTCollections.Include(n => n.Category).Where(n => n.CreatorId == id),
                NFTs = _context.NFTs.Include(n => n.Metadata).ThenInclude(n => n.Creator).Include(n=>n.Owner).Where(n => n.OwnerId == user.NFTUserId || n.Metadata.CreatorId == user.NFTUserId),
                Sales = _context.Sales.Include(s => s.From).Include(s => s.To).Include(s => s.NFT).Include(s => s.Collection).Where(s => s.From == user.NFTUser),
                Purchases = _context.Sales.Include(s=>s.From).Include(s=>s.To).Include(s => s.NFT).Include(s => s.Collection).Where(s => s.To == user.NFTUser),
                User = user
                
            };
            return View(details);
        }
    }
}
