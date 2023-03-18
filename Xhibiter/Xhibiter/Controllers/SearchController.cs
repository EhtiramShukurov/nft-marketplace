using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.ViewModels;

namespace Xhibiter.Controllers
{
    public class SearchController : Controller
    {
        AppDbContext _context { get; }

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? query)
        {
            SearchVM search = new();
            search.Collections = _context.NFTCollections.Include(n => n.Creator).ThenInclude(n => n.NFTUser).Include(n=>n.NFTs).Include(n=>n.Category).Where(n => n.Name.Contains(query) || n.Category.Name.Contains(query));
            search.NFTs = _context.NFTs.Include(n => n.Metadata).Include(n=>n.NFTCategories).ThenInclude(n=>n.Category).Where(n =>n.IsSale && n.Name.Contains(query)|| n.IsSale && n.NFTCategories.Any(n=>n.Category.Name.Contains(query)));
            search.UserProfiles = _context.UserProfiles.Include(u=>u.NFTUser).Where(u => u.NFTUser.UserName.Contains(query));
            return View(search);
        }
    }
}
