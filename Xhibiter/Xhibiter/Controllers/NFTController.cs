using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.Utilities;
using Xhibiter.ViewModels;

namespace Xhibiter.Controllers
{
    public class NFTController : Controller
    {
        UserManager<NFTUser> _userManager { get; }
        AppDbContext _context { get;}
        IWebHostEnvironment _env { get; }
        public NFTController(AppDbContext context, IWebHostEnvironment env, UserManager<NFTUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        public IActionResult Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            NFT nft = _context.NFTs.Include(n=>n.Metadata).Include(n=>n.Sales).Include(n=>n.Auction).ThenInclude(a=>a.Bids).FirstOrDefault(n=>n.Id==id);
            if (nft is null) return NotFound();
            _context.NFTs.Remove(nft);
            nft.Metadata.MediaUrl.DeleteFile(_env.WebRootPath, "assets/media/nft");

            _context.NFTMetadatas.Remove(nft.Metadata);
            _context.Bids.RemoveRange(nft.Auction.Bids);
            _context.Auctions.Remove(nft.Auction);
            _context.Sales.RemoveRange(nft.Sales);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public  async Task<IActionResult> Index(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            NFT nft = _context.NFTs.Include(n=>n.Metadata).ThenInclude(n=>n.Creator).ThenInclude(c=>c.Profile)
                .Include(n=>n.Owner).ThenInclude(n=>n.Profile).FirstOrDefault(n => n.Id == id);
            if (nft is null) return NotFound();
            var auction = _context.Auctions.Include(n=>n.Winner).ThenInclude(w=>w.NFTUser).FirstOrDefault(a => a.NFTId == id);
            var colname = _context.NFTCollections.FirstOrDefault(n=>n.Id == nft.CollectionId).Name;
            NFTDetailVM NFTDetail = new()
            {
                CollectionName = colname,
                NFT = nft,
                NFTs = _context.NFTs.Include(n=>n.Metadata).Where(n => n.CollectionId == nft.CollectionId && n.Id != nft.Id).Where(n=>n.IsSale),
                Auction = auction,
                Bids = _context.Bids.Include(b=>b.Bidder).ThenInclude(b=>b.NFTUser).Where(b => auction != null && b.AuctionId == auction.Id).OrderByDescending(b => b.Amount),
                Sales = _context.Sales.Include(s=>s.From).Include(s=>s.To).Where(s => s.NFTId == id),
            };
            if (auction != null)
            {
                await nft.CompleteAuction(auction,_context);
            }
            return View(NFTDetail);
        }
        public IActionResult FilterByCategory(string categoryName)
        {
            var nfts = _context.NFTs.Include(n => n.Metadata).Include(n => n.Auction).Include(n=>n.NFTCategories).Where(n => n.NFTCategories
                                 .Any(nc => nc.Category.Name == categoryName && n.IsSale)).Take(20);
            if (categoryName == "All")
            {
                nfts = _context.NFTs.Include(n => n.Metadata).ThenInclude(m => m.Creator).ThenInclude(c => c.Profile).Include(n => n.Auction).Where(n=>n.IsSale);
            }

            return PartialView("_OnSalePartial", nfts);
        }
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
            ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateNFTVM createNFT)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            }
            if (_context.NFTs.Any(n => n.Name == createNFT.Name))
            {
                ModelState.AddModelError("Name", "There is already an nft with this name!");
                ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            }
            IFormFile media = createNFT.Media;
            var res = media.ContentType;
                string result = media.CheckValidate(30);
                if (result.Length > 0)
                {
                    ModelState.AddModelError("Media", result);
                    ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                    ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                    return View();
            }
            if (!_context.NFTCollections.Any(c => c.Id == createNFT.CollectionId))
            {
                ModelState.AddModelError("CollectionIds", "There is no matched collection with this id!");
                ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            }
            foreach (int categoryId in (createNFT.CategoryIds ?? new List<int>()))
            {
                if (!_context.Categories.Any(c => c.Id == categoryId))
                {
                    ModelState.AddModelError("CategoryIds", "There is no matched category with this id!");
                    ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                    ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                    return View();
                }
            }
            var categories = _context.Categories.Where(ca => createNFT.CategoryIds.Contains(ca.Id));
            NFTUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null) return BadRequest();
            NFTMetadata metadata = new NFTMetadata()
            {
                TokenId = "",
                ExternalLink = createNFT.ExternalLink,
                Creator = user,
                CreatedDate = DateTime.Now,
            };
            if (res.Contains("video/"))
            {
                metadata.MediaType = "video";
                metadata.MediaUrl = media.SaveFile(Path.Combine(_env.WebRootPath, "assets", "media", "nft"));
            }
            else if (res.Contains("audio/"))
            {
                metadata.MediaType = "audio";
                metadata.MediaUrl = media.SaveFile(Path.Combine(_env.WebRootPath, "assets", "media", "nft"));

            }
            else
            {
                metadata.MediaType = "image";
                metadata.MediaUrl = media.SaveImage(Path.Combine(_env.WebRootPath, "assets", "media", "nft"));

            }
            await _context.NFTMetadatas.AddAsync(metadata);
            NFT nft = new NFT
            {
                Owner = user,
                Name = createNFT.Name,
                Description = createNFT.Description,
                Price = createNFT.Price,
                IsSale = createNFT.IsSale,
                Metadata = metadata,
                CollectionId = createNFT.CollectionId
            };
            await _context.NFTs.AddAsync(nft);
            foreach (var item in categories)
            {
                await _context.NFTCategories.AddAsync(new NFTCategory { NFT = nft, CategoryId = item.Id });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            NFT exist = await _context.NFTs.Include(n=>n.Metadata).Include(n=>n.NFTCategories)
                .FirstOrDefaultAsync(n=>n.Id == id);
            if (exist is null) return NotFound();
            EditNFTVM edit = new EditNFTVM()
            {
                Name = exist.Name,
                Description = exist.Description,
                CategoryIds = exist.NFTCategories.Select(n => n.CategoryId).ToList<int>(),
                CollectionId = exist.CollectionId,
                ExternalLink = exist.Metadata.ExternalLink,
                IsSale = exist.IsSale,
                Price = exist.Price,
            };
            ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
            ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
            ViewBag.Image = exist.Metadata.MediaUrl;
            return View(edit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int? id,EditNFTVM editNFT)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                ViewBag.Image =  _context.NFTs.Include(n => n.Metadata).FirstOrDefault(n => n.Id == id).Metadata.MediaUrl;
                return View();
            }
            if (_context.NFTs.Any(n=>n.Name == editNFT.Name && n.Id != id))
            {
                ModelState.AddModelError("Name", "There is already an nft with this name!");
                ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                ViewBag.Image = _context.NFTs.Include(n => n.Metadata).FirstOrDefault(n => n.Id == id).Metadata.MediaUrl;
                return View();
            }
            var media = editNFT.Media;
            if (media != null)
            {
                var res = media.ContentType;
                string result = media.CheckValidate(30);
                if (result.Length > 0)
                {
                    ModelState.AddModelError("Media", result);
                    ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                    ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                    ViewBag.Image = _context.NFTs.Include(n => n.Metadata).FirstOrDefault(n => n.Id == id).Metadata.MediaUrl;
                    return View();
                }
            }
            if (!_context.NFTCollections.Any(c => c.Id == editNFT.CollectionId))
            {
                ModelState.AddModelError("CollectionId", "There is no matched collection with this id!");
                ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                ViewBag.Image = _context.NFTs.Include(n=>n.Metadata).FirstOrDefault(n => n.Id == id).Metadata.MediaUrl;
                return View();
            }

            foreach (int categoryId in (editNFT.CategoryIds ?? new List<int>()))
            {
                if (!_context.Categories.Any(c => c.Id == categoryId))
                {
                    ModelState.AddModelError("CategoryIds", "There is no matched category with this id!");
                    ViewBag.Collections = new SelectList(_context.NFTCollections.Where(n => n.Creator.NFTUser.UserName == User.Identity.Name), nameof(NFTCollection.Id), nameof(NFTCollection.Name));
                    ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                    ViewBag.Image = _context.NFTs.Include(n => n.Metadata).FirstOrDefault(n => n.Id == id).Metadata.MediaUrl;
                    return View();
                }
            }
            NFT exist = await _context.NFTs.Include(n => n.Metadata).Include(n => n.NFTCategories)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (exist is null) return NotFound();
            if (media != null)
            {
                var res = media.ContentType;
                exist.Metadata.MediaUrl.DeleteFile(_env.WebRootPath, "assets/media/nft");
                if (res.Contains("video/"))
                {
                    exist.Metadata.MediaType = "video";
                    exist.Metadata.MediaUrl = media.SaveFile(Path.Combine(_env.WebRootPath, "assets", "media", "nft"));
                }
                else if (res.Contains("audio/"))
                {
                    exist.Metadata.MediaType = "audio";
                    exist.Metadata.MediaUrl = media.SaveFile(Path.Combine(_env.WebRootPath, "assets", "media", "nft"));

                }
                else
                {
                    exist.Metadata.MediaType = "image";
                    exist.Metadata.MediaUrl = media.SaveImage(Path.Combine(_env.WebRootPath, "assets", "media", "nft"));

                }
            }

            foreach (var item in exist.NFTCategories)
            {
                if (editNFT.CategoryIds.Contains(item.CategoryId))
                {
                    editNFT.CategoryIds.Remove(item.CategoryId);
                }
                else
                {
                    _context.NFTCategories.Remove(item);
                }
            }
            foreach (var categoryId in editNFT.CategoryIds)
            {
                await _context.NFTCategories.AddAsync(new NFTCategory { NFT = exist, CategoryId = categoryId });
            }
            exist.Name = editNFT.Name;
            exist.Description = editNFT.Description;
            exist.Metadata.ExternalLink = editNFT.ExternalLink;
            exist.CollectionId = editNFT.CollectionId;
            exist.IsSale = editNFT.IsSale;
            exist.Price = editNFT.Price;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index",new {id = exist.Id});
        }
    }
}
