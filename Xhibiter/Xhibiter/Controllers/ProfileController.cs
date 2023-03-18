using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.Utilities;
using Xhibiter.ViewModels;

namespace Xhibiter.Controllers
{
    public class ProfileController : Controller
    {
        UserManager<NFTUser> _userManager { get; }
        SignInManager<NFTUser> _signinManager { get; }
        AppDbContext _context { get; }
        IWebHostEnvironment _env { get; }

        public ProfileController(AppDbContext context, IWebHostEnvironment env, UserManager<NFTUser> userManager, SignInManager<NFTUser> signinManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _signinManager = signinManager;
        }
        public async Task<IActionResult> Delete(string? username)
        {
            if (username is null || username.Length == 0 || username != User.Identity.Name) return BadRequest();
            UserProfile profile =await _context.UserProfiles.Include(u => u.NFTUser).Include(n=>n.Collections).FirstOrDefaultAsync(x => x.NFTUser.UserName == username);
            if (profile is null) return NotFound();
            profile.ProfileImageUrl.DeleteFile(_env.WebRootPath, $"assets/images/users/{User.Identity.Name}/profile");
            profile.CoverImageUrl.DeleteFile(_env.WebRootPath, $"assets/images/users/{User.Identity.Name}/cover");
            UserProfile admin = await _context.UserProfiles.Include(n=>n.NFTUser).FirstOrDefaultAsync(u => u.NFTUser.UserName == "admin");
            if (admin is null) return NotFound();
            var nfts = _context.NFTs.Include(n=>n.Metadata).Where(n => n.Metadata.CreatorId == profile.NFTUserId && n.OwnerId != profile.NFTUserId);
                foreach (var item in nfts)
                {
                item.Metadata.CreatorId = admin.NFTUserId;
                }
            foreach (var item in profile.Collections)
            {
                item.CreatorId = admin.Id;
            }
            _context.UserProfiles.Remove(profile);
            _context.NFTUsers.Remove(profile.NFTUser);
            await _context.SaveChangesAsync();
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Index(string? username)
        {
            if (username is null || username.Length == 0) return BadRequest();
            var user = _context.UserProfiles.Include(n => n.Collections).Include(u => u.NFTUser).Include(u => u.Bids).ThenInclude(b => b.Auction).FirstOrDefault(n => n.NFTUser.UserName == username);
            if (user is null) return NotFound();
            var OwnedNFTs = _context.NFTs.Include(n => n.Metadata).Include(n => n.Auction).Where(n => n.OwnerId == user.NFTUserId);
            var created = _context.NFTs.Include(n => n.Metadata).Include(n => n.Auction).Where(o => o.Metadata.CreatorId == user.NFTUserId && o.IsSale == true);
            var hidden = OwnedNFTs.Where(n => !n.IsSale && !n.IsAuctioned);
            var auctioned = OwnedNFTs.Where(n => !n.IsSale && n.IsAuctioned);
            var onSale = OwnedNFTs.Where(n => n.IsSale && !n.IsAuctioned);
            var collections = user.Collections;
            var activities = _context.Activities.Include(a => a.Bid.Auction.NFT).Include(a => a.Sale.From.Profile)
                .Include(a => a.Sale.To.Profile).Include(a=>a.Sale.NFT)
                .Where(a => a.SaleId != null && a.Sale.From.Profile.Id == user.Id || a.SaleId != null && a.Sale.To.Profile.Id == user.Id||a.BidId != null &&  a.Bid.BidderId == user.Id ).OrderByDescending(a=>a.Date);
            ProfileDetailsVM profile = new()
            {
                Activities = activities,
             AuctionedNFTs = auctioned,
             Collections = collections,
             HiddenNFTs = hidden,
             OnSaleNFTs = onSale,
             User = user,
             CreatedNFTs = created
            };
            return View(profile);
        }
        [Authorize]

        public IActionResult Edit(string? username)
        {
            if (username is null || username.Length == 0||username != User.Identity.Name) return BadRequest();
            UserProfile profile = _context.UserProfiles.Include(u => u.NFTUser).FirstOrDefault(x => x.NFTUser.UserName == username);
            if (profile is null) return NotFound();
            EditProfileVM editProfile = new EditProfileVM()
            {
                Bio = profile.Bio,
                CoverImgUrl = profile.CoverImageUrl,
                ProfileImgUrl = profile.ProfileImageUrl,
                TwitterLink = profile.TwitterLink,
                InstagramLink = profile.InstagramLink,
                ExternalLink = profile.ExternalLink
            };

            return View(editProfile);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileVM editProfile)
        {
            string name = User.Identity.Name;
            if (name is null || name.Length == 0) return BadRequest();
            var userr = await _userManager.FindByNameAsync(name);
            var coverImg = editProfile.CoverImg;
            var profileimg = editProfile.ProfileImg;
            var result = coverImg?.CheckValidate2(9);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("", result);
            }
            result = profileimg?.CheckValidate2(9);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("", result);
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            UserProfile profile =await _context.UserProfiles.Include(u => u.NFTUser).FirstOrDefaultAsync(x => x.NFTUser.UserName == name);
            if (profile is null) return NotFound();
            if (coverImg != null)
            {
                profile.CoverImageUrl?.DeleteFile(_env.WebRootPath, $"assets/images/users/{name}/cover");
                var newCover = coverImg.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "users",User.Identity.Name,"cover"));
                profile.CoverImageUrl = newCover;
            }
            if (profileimg != null)
            {
                profile.ProfileImageUrl?.DeleteFile(_env.WebRootPath, $"assets/images/users/{name}/profile");
                var newProfile = profileimg.SaveImage(Path.Combine(_env.WebRootPath, "assets", "images", "users", User.Identity.Name, "profile"));
                profile.ProfileImageUrl = newProfile;
            }
            name = editProfile.Username;
            profile.Bio = editProfile.Bio;
            userr.UserName = editProfile.Username;
            profile.TwitterLink = editProfile.TwitterLink;
            profile.InstagramLink = editProfile.InstagramLink;
            profile.ExternalLink = editProfile.ExternalLink;
            await _userManager.UpdateAsync(userr);
            await _context.SaveChangesAsync();
            await _signinManager.SignInAsync(userr, false);
            return RedirectToAction(nameof(Index),new {username = name });
        }
    }
}
