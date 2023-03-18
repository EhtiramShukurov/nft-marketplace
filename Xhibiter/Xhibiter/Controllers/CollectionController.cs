using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.Utilities;
using Xhibiter.ViewModels;

namespace Xhibiter.Controllers
{
    public class CollectionController : Controller
    {
        AppDbContext _context { get; }
        IWebHostEnvironment _env { get; }
        UserManager<NFTUser> _userManager { get; }

        public CollectionController(AppDbContext context, IWebHostEnvironment env, UserManager<NFTUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        public IActionResult Explore()
        {
            var collections = _context.NFTCollections.Include(n => n.Creator).ThenInclude(n => n.NFTUser).Include(n => n.NFTs);
            ViewBag.Categories = _context.Categories.ToList();
            return View(collections);
        }

        public ActionResult FilterByCategory(string categoryName)
        {
            var collections = _context.NFTCollections.Include(n => n.Creator).ThenInclude(n => n.NFTUser).Include(n => n.NFTs).Where(c => c.Category.Name == categoryName);
            if (categoryName == "All")
            {
                collections = _context.NFTCollections.Include(n => n.Creator).ThenInclude(n => n.NFTUser).Include(n => n.NFTs);
            }

            return PartialView("_CollectionsPartial", collections);
        }
        public IActionResult Index(int? id)
        {
            NFTCollection collection = _context.NFTCollections.Include(n=>n.NFTs.Where(n => n.IsSale || n.IsAuctioned)).ThenInclude(n=>n.Owner).ThenInclude(n=>n.Profile)
                .Include(n => n.NFTs.Where(n=>n.IsSale || n.IsAuctioned)).ThenInclude(n=>n.Metadata).ThenInclude(m=>m.Creator)
                .Include(n => n.Creator).ThenInclude(c=>c.NFTUser).FirstOrDefault(n => n.Id == id);
            if (User.Identity.IsAuthenticated && User.Identity.Name == collection.Creator.NFTUser.UserName)
            {
                collection = _context.NFTCollections.Include(n => n.NFTs).ThenInclude(n => n.Owner).ThenInclude(n => n.Profile)
                .Include(n => n.NFTs).ThenInclude(n => n.Metadata).ThenInclude(m => m.Creator)
                .Include(n => n.Creator).ThenInclude(c => c.NFTUser).FirstOrDefault(n => n.Id == id);
            }
                return View(collection);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCollectionVM collection)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            }
            if (_context.NFTCollections.Any(n => n.Name == collection.Name))
            {
                ModelState.AddModelError("Name", "This name is already taken!");
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            }
            IFormFile cover = collection.CoverImg;
            IFormFile logo = collection.LogoImg;
            IFormFile main = collection.MainImg;
            string result = cover.CheckValidate2(19);
            if (result.Length > 0)
            {
                ModelState.AddModelError("CoverImg", result);
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            };
            result = logo.CheckValidate2(10);
            if (result.Length > 0)
            {
                ModelState.AddModelError("LogoImg", result);
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            };
            result = logo.CheckValidate2(10);
            if (result.Length > 0)
            {
                ModelState.AddModelError("MainImg", result);
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            };
            if (!_context.Categories.Any(c=>c.Id == collection.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "There is no category in this id!");
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            };
            char[] disallowedChars = new char[] { '#', '*', '$','/',':','<','>','?','|'};

            if (collection.Name.IndexOfAny(disallowedChars) != -1)
            {
                ModelState.AddModelError("Name", " Collection name can't contain *,#,$,/,:,<,>,?,|");
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                return View();
            }
            UserProfile user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.NFTUser.UserName == User.Identity.Name);
            if (user == null) return BadRequest();
            NFTCollection nFTCollection = new NFTCollection()
            {
                Name = collection.Name,
                Description = collection.Description,
                CreationDate = DateTime.Now,
                Creator = user,
                CategoryId = collection.CategoryId,
                CoverImgUrl = cover.SaveImage(Path.Combine(_env.WebRootPath, "assets", "images", "users",User.Identity.Name,"collections",collection.Name)),
                LogoImgUrl = logo.SaveImage(Path.Combine(_env.WebRootPath, "assets", "images", "users",User.Identity.Name,"collections",collection.Name)),
                MainImgUrl = main.SaveImage(Path.Combine(_env.WebRootPath, "assets", "images", "users",User.Identity.Name,"collections",collection.Name)),
            };
            await _context.NFTCollections.AddAsync(nFTCollection);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Profile",new {username = User.Identity.Name});
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            NFTCollection exist = await _context.NFTCollections.FirstOrDefaultAsync(n => n.Id == id);
            if (exist is null) return NotFound();
            EditCollectionVM edit = new EditCollectionVM()
            {
                Id = exist.Id,
                Name = exist.Name,
                Description = exist.Description,
                CategoryId = exist.CategoryId,
            };
            ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
            ViewBag.Imgs = new string[] { exist.MainImgUrl, exist.LogoImgUrl, exist.CoverImgUrl };
            return View(edit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, EditCollectionVM editCollection)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                var existColl = await _context.NFTCollections.FirstOrDefaultAsync(n => n.Id == id);
                ViewBag.Imgs = new string[] { existColl.MainImgUrl, existColl.LogoImgUrl, existColl.CoverImgUrl };
                EditCollectionVM edit = new EditCollectionVM()
                {
                    Id = existColl.Id,
                    Name = existColl.Name,
                    Description = existColl.Description,
                    CategoryId = existColl.CategoryId,
                };
                return View(edit);
            }
            if (_context.NFTCollections.Any(n => n.Name == editCollection.Name && n.Id != editCollection.Id))
            {
                ModelState.AddModelError("Name", "This name is already taken!");
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                var existColl = await _context.NFTCollections.FirstOrDefaultAsync(n => n.Id == id);
                ViewBag.Imgs = new string[] { existColl.MainImgUrl, existColl.LogoImgUrl, existColl.CoverImgUrl };
                EditCollectionVM edit = new EditCollectionVM()
                {
                    Id = existColl.Id,
                    Name = existColl.Name,
                    Description = existColl.Description,
                    CategoryId = existColl.CategoryId,
                };
                return View(edit);
            }
            var cover = editCollection.CoverImg;
            var main = editCollection.MainImg;
            var logo = editCollection.LogoImg;
            string result = cover?.CheckValidate(10);
            if (cover != null)
            {
                if (result.Length > 0)
                {
                    ModelState.AddModelError("CoverImg", result);
                    ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                    var existColl = await _context.NFTCollections.FirstOrDefaultAsync(n => n.Id == id);
                    ViewBag.Imgs = new string[] { existColl.MainImgUrl, existColl.LogoImgUrl, existColl.CoverImgUrl };
                    EditCollectionVM edit = new EditCollectionVM()
                    {
                        Id = existColl.Id,
                        Name = existColl.Name,
                        Description = existColl.Description,
                        CategoryId = existColl.CategoryId,
                    };
                    return View(edit);
                }
            }
            if (main != null)
            {
                 result =  main.CheckValidate(10);

                if (result.Length > 0)
                {
                    ModelState.AddModelError("MainImg", result);
                    ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                    var existColl = await _context.NFTCollections.FirstOrDefaultAsync(n => n.Id == id);
                    ViewBag.Imgs = new string[] { existColl.MainImgUrl, existColl.LogoImgUrl, existColl.CoverImgUrl };
                    EditCollectionVM edit = new EditCollectionVM()
                    {
                        Id = existColl.Id,
                        Name = existColl.Name,
                        Description = existColl.Description,
                        CategoryId = existColl.CategoryId,
                    };
                    return View(edit);
                }
            }
            if (logo != null)
            {
                 result = logo.CheckValidate(10);

                if (result.Length > 0)
                {
                    ModelState.AddModelError("LogoImg", result);
                    ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                    var existColl = await _context.NFTCollections.FirstOrDefaultAsync(n => n.Id == id);
                    ViewBag.Imgs = new string[] { existColl.MainImgUrl, existColl.LogoImgUrl, existColl.CoverImgUrl };
                    EditCollectionVM edit = new EditCollectionVM()
                    {
                        Id = existColl.Id,
                        Name = existColl.Name,
                        Description = existColl.Description,
                        CategoryId = existColl.CategoryId,
                    };
                    return View(edit);
                }
            }
            if (!_context.Categories.Any(c => c.Id == editCollection.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "There is no matched category with this id!");
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                var existColl = await _context.NFTCollections.FirstOrDefaultAsync(n => n.Id == id);
                ViewBag.Imgs = new string[] { existColl.MainImgUrl, existColl.LogoImgUrl, existColl.CoverImgUrl };
                EditCollectionVM edit = new EditCollectionVM()
                {
                    Id = existColl.Id,
                    Name = existColl.Name,
                    Description = existColl.Description,
                    CategoryId = existColl.CategoryId,
                };
                return View(edit);
            }
            NFTCollection exist = await _context.NFTCollections.Include(n=>n.Creator).FirstOrDefaultAsync(n => n.Id == id);
            if (exist is null) return NotFound();
            if (cover != null)
            {
                exist.CoverImgUrl.DeleteFile(_env.WebRootPath, $"assets/images/users/{User.Identity.Name}/collections/{exist.Name}");
                exist.CoverImgUrl = cover.SaveImage(Path.Combine(_env.WebRootPath, "assets", "images", "users",User.Identity.Name,"collections",editCollection.Name));
            }
            if (main != null)
            {
                exist.MainImgUrl.DeleteFile(_env.WebRootPath, $"assets/images/users/{User.Identity.Name}/collections/{exist.Name}");
                exist.MainImgUrl = main.SaveImage(Path.Combine(_env.WebRootPath, "assets", "images", "users", User.Identity.Name, "collections", editCollection.Name));
            }
            if (logo != null)
            {
                exist.LogoImgUrl.DeleteFile(_env.WebRootPath, $"assets/images/users/{User.Identity.Name}/collections/{exist.Name}");
                exist.LogoImgUrl = logo.SaveImage(Path.Combine(_env.WebRootPath, "assets", "images", "users", User.Identity.Name, "collections", editCollection.Name));
            }
            exist.Name = editCollection.Name;
            exist.Description = editCollection.Description;
            exist.CategoryId = editCollection.CategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = exist.Id });
        }
    }
}
