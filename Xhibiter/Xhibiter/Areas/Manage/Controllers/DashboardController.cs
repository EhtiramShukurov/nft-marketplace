using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.Areas.Manage.ViewModels;
using Xhibiter.DAL;

namespace Xhibiter.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles ="Admin")]
    public class DashboardController : Controller
    {
        AppDbContext _context { get; }

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            DashboardVM dashboard = new()
            {
                CollectionNumber = _context.NFTCollections.Count(),
                SaleNumber = _context.Sales.Count(),
                UserNumber = _context.NFTUsers.Count(),
                NFTNumber = _context.NFTs.Count(),
                Sales = _context.Sales.Include(s => s.Collection).Include(s => s.NFT).Include(s=>s.From).Include(s=>s.To).OrderByDescending(m=>m.Date).Take(10),
            };
            return View(dashboard);
        }
    }
}
